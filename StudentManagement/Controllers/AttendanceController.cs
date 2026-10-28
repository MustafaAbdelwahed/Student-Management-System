using CompanySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.DTO.UserCreatedDto;
using StudentManagement.DTOs.AttendanceDtos;
using StudentManagement.Model;
using System.Security.Claims;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {

        private readonly AppDbContext _db;

        public AttendanceController(AppDbContext db) => _db = db;

        private bool IsStaff => User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Teacher);

        // Teacher/Admin: mark (or correct) a whole class for one day
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpPost("course/{courseId}")]
        public async Task<IActionResult> Mark(int courseId, MarkAttendanceDto dto)
        {
            // 1. there must be at least one record
            if (dto.Records is null || dto.Records.Count == 0)
                return BadRequest(new { message = "Send at least one attendance record." });

            // 2. the date must be a real date (an unsent date becomes 0001-01-01)
            if (dto.Date == default)
                return BadRequest(new { message = "A valid date is required." });

            // 3. no blank student ids
            if (dto.Records.Any(r => string.IsNullOrWhiteSpace(r.StudentId)))
                return BadRequest(new { message = "Every record needs a studentId." });

            // 4. convert each status text to the enum and reject anything unknown
            var parsed = new List<(string StudentId, AttendanceStatus Status)>();
            var invalid = new List<string>();

            foreach (var Record in dto.Records)
            {
                if (Enum.TryParse<AttendanceStatus>(Record.Status, ignoreCase: true, out var status)
                    && Enum.IsDefined(status))
                {
                    parsed.Add((Record.StudentId, status));
                }
                else
                {
                    invalid.Add(Record.StudentId);
                }
            }

            if (invalid.Count > 0)
                return BadRequest(new
                {
                    message = $"Invalid status. Allowed values: {string.Join(", ", Enum.GetNames<AttendanceStatus>())}.",
                    studentIds = invalid
                });

            // 5. the course must exist
            if (!await _db.Courses.AnyAsync(c => c.Id == courseId))
                return NotFound("Course not found.");

            // if the same student appears twice, the last entry wins
            var records = parsed
            .GroupBy(p => p.StudentId)
            .Select(g => g.Last())
            .ToList();

            var studentIds = records.Select(r => r.StudentId).ToList();

            // 6. every student must be enrolled in this course
            var enrolledIds = await _db.enrollments
                .Where(e => e.CourseId == courseId && studentIds.Contains(e.StudentId))
                .Select(e => e.StudentId)
                .ToListAsync();

            var notEnrolled = studentIds.Except(enrolledIds).ToList();
            if (notEnrolled.Count > 0)
                return BadRequest(new
                {
                    message = "Some students are not enrolled in this course.",
                    studentIds = notEnrolled
                });

            // rows that already exist for this day get updated, the rest are added
            var existing = await _db.Attendances
                .Where(a => a.CourseId == courseId && a.Date == dto.Date && studentIds.Contains(a.StudentId))
                .ToListAsync();


            foreach (var record in records)
            {

                var row = existing.FirstOrDefault(a => a.StudentId == record.StudentId);

                if (row is null)
                {
                    _db.Attendances.Add(new Attendance
                    {
                        StudentId = record.StudentId,
                        CourseId = courseId,
                        Date = dto.Date,
                        Status = record.Status          
                    });
                }
                else
                {
                    return BadRequest(new { message = "you Entered Attend in same date Before !"});
                }
            }
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // e.g. two teachers saved the same class at the same moment (unique index)
                return Conflict(new { message = "Attendance could not be saved. Please try again." });
            }

            return NoContent();
        }

        // Teacher/Admin: a course's attendance. Add ?date=2026-10-23 for one day.
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<GetAttendanceDataDto>>> GetByCourse(int courseId,  DateOnly? date)
        {
            if (!await _db.Courses.AnyAsync(c => c.Id == courseId))
                return NotFound("Course not found.");

            var list = await _db.Attendances
                .AsNoTracking()
                .Where(a => a.CourseId == courseId && (date == null || a.Date == date))
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Student.FullName)
                .Select(a => new GetAttendanceDataDto
                {
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    CourseId = a.CourseId,
                    CourseTitle = a.Course.Title,
                    Date = a.Date,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        // Staff: any student's history. Student: only their own. Optional ?courseId=

        [HttpGet("student/{studentId}")]

        public async Task<ActionResult<List<GetAttendanceDataDto>>> GetByStudent(string studentId, [FromQuery] int courseId)
        {

            if (!IsStaff && studentId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            var list = await _db.Attendances
                .AsNoTracking()
                .Where(a => a.StudentId == studentId && ( a.CourseId == courseId))
                .OrderByDescending(a => a.Date)
                .Select(a => new GetAttendanceDataDto
                {
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    CourseId = a.CourseId,
                    CourseTitle = a.Course.Title,
                    Date = a.Date,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(list);
        }


        //[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpPut("course/{courseId}/student/{studentId}")]
        public async Task<IActionResult> EditStatus(int courseId, string studentId,  EditAttendanceDto editAttendanceDto)
        //public async Task<IActionResult> EditStatus(int courseId, string studentId, [FromQuery] DateOnly date, [FromBody] string status)
        {
            if (!Enum.TryParse<AttendanceStatus>(editAttendanceDto. status, ignoreCase: true, out var newStatus) || !Enum.IsDefined(newStatus))
                return BadRequest(new
                {
                    message = $"Invalid status. Allowed values: {string.Join(", ", Enum.GetNames<AttendanceStatus>())}."
                });

            var row = await _db.Attendances
                .FirstOrDefaultAsync(a => a.CourseId == courseId && a.StudentId == studentId && a.Date == editAttendanceDto. date);

            if (row is null)
                return NotFound("No attendance record for this student on this date.");

            row.Status = newStatus;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Student shortcut: my attendance history
        [Authorize(Roles = Roles.Student)]
        [HttpGet("me")]
        public async Task<ActionResult<List<GetAttendanceDataDto>>> GetMine([FromQuery] int courseId)
        {
            var myId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (myId is null) return Unauthorized();

            var list = await _db.Attendances
                .AsNoTracking()
                .Where(a => a.StudentId == myId && (courseId == null || a.CourseId == courseId))
                .OrderByDescending(a => a.Date)
                .Select(a => new GetAttendanceDataDto
                {
                    StudentId = a.StudentId,
                    StudentName = a.Student.FullName,
                    CourseId = a.CourseId,
                    CourseTitle = a.Course.Title,
                    Date = a.Date,
                    Status = a.Status
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
