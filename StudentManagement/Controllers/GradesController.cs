using CompanySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.DTOs.GradeDto;
using StudentManagement.Model;
using System.Security.Claims;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class GradesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public GradesController(AppDbContext db) => _db = db;

        private bool IsStaff => User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Teacher);

   

        // Teacher/Admin: add or edit a grade
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpPut("{studentId}/{courseId}")]
        public async Task<IActionResult> SetGrade(string studentId, int courseId, SetGradeDto dto)
        {
            Enrollment? enrollment = await _db.enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment is null)
                return NotFound("This student is not enrolled in this course.");

            enrollment.Grade = dto.Grade;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Teacher/Admin: all grades in one course
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<GetGradeDto>>> GetByCourse(int courseId)
        {
            if (!await _db.Courses.AnyAsync(c=>c.Id==courseId))
                return NotFound("Course not found.");
            
            List<GetGradeDto> dtos = await _db.enrollments.Where(c => c.CourseId == courseId).Select(c => new GetGradeDto
            {
                CourseId = c.CourseId,
                CourseTitle = c.Course.Title,
                StudentId = c.StudentId,
                StudentName = c.Student.FullName,
                Grade = c.Grade
            }).ToListAsync();

            if (dtos.Count==0)
            {
                return NotFound("There is no Student in This Course");
            }
            return Ok(dtos);
        }


        // Staff: any student's grades. Student: only their own.
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]

        public async Task<ActionResult<List<GetGradeDto>>> GetByStudentID(string studentId)
        {

            if (!IsStaff && studentId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            if (!await _db.Students.AnyAsync(s => s.Id == studentId))
                return NotFound("Student not found.");

            List<GetGradeDto> gradeDtos=await _db.enrollments.Where(e => e.StudentId == studentId).Select(e => new GetGradeDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student.FullName,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                Grade = e.Grade
            }).ToListAsync();

            if (gradeDtos.Count == 0)
            {
                return NotFound("Tha student whit this ID is not enroll in this Course ");
            }
            return Ok(gradeDtos);
        }

        // Student shortcut: my grades
        [Authorize(Roles = Roles.Student)]
        [HttpGet("me")]
        public async Task<ActionResult<List<GetGradeForStdDto>>> GetMine()
        {
            var myId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (myId is null) return Unauthorized();

            List<GetGradeForStdDto> gradeDtos = await _db.enrollments.Where(e => e.StudentId == myId).Select(e => new GetGradeForStdDto
            {
                StudentName = e.Student.FullName,
                CourseTitle = e.Course.Title,
                Grade = e.Grade
            }).ToListAsync();

            return Ok( gradeDtos);
        }
    }
}
