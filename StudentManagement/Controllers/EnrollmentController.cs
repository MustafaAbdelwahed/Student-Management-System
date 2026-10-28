using CompanySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.DTO.EnrollmentCreateDto;
using StudentManagement.DTO.EnrollmentDtos;
using StudentManagement.DTO.EnrollmentGetDataDto;
using StudentManagement.Model;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        public EnrollmentController(AppDbContext mDBcontext)
        {
            _dbcontext = mDBcontext;
        }
        // GET api/enrollments
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet]
        public async Task<IActionResult> GetAllEnrollments()
        {

            List<EnrollmentGetDataDto> enrollments = await _dbcontext.enrollments.Select(e => new EnrollmentGetDataDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student.FullName,
                CourseId = e.CourseId,
                Grade = e.Grade,
                CourseTitle = e.Course.Title,
                classe = e.Course.Classe,
                teacher = e.Course.Teacher
            })

            .ToListAsync    ();

            return Ok(enrollments);
        }

        // GET api/enrollments/student/5/course/3
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]

        [HttpGet("student/{studentId}/course/{courseId}")]
        public async Task<IActionResult> GetEnrollment(string studentId, int courseId)
        {

            EnrollmentGetDataDto? enrollment = await _dbcontext.enrollments
         .Include(e => e.Course).Include(e => e.Student)
         .Select(e => new EnrollmentGetDataDto
         {
             StudentId = e.StudentId,
             StudentName = e.Student.FullName,
             CourseId = e.CourseId,
             CourseTitle = e.Course.Title,
             Grade = e.Grade,
             teacher = e.Course.Teacher,
             classe = e.Course.Classe
         })
         .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
            {
                return NotFound();
            }



            return Ok(enrollment);
        }

        // GET api/enrollments/student/5  -> all courses of a student

        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetEnrollmentsByStudent(string studentId)
        {
            if (!await _dbcontext.Students.AnyAsync(s => s.Id == studentId))
            {
                return NotFound("Student not found.");
            }

            List<EnrollmentGetDataDto> enrollments = await  _dbcontext.enrollments
                .Include(e => e.Course).Include(e => e.Student)
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentGetDataDto
                {
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    Grade = e.Grade,
                    teacher = e.Course.Teacher,
                    classe = e.Course.Classe
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // GET api/enrollments/course/3  -> all students in a course

        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetEnrollmentsByCourse(int courseId)
        {
            if (! await   _dbcontext.Courses.AnyAsync(c => c.Id == courseId))
            {
                return NotFound("Course not found.");
            }

            List<EnrollmentGetDataDto> enrollments =await _dbcontext.enrollments
              .Include(e => e.Course).Include(e => e.Student)
              .Where(e => e.CourseId == courseId)
              .Select(e => new EnrollmentGetDataDto
              {
                  StudentId = e.StudentId,
                  StudentName = e.Student.FullName,
                  CourseId = e.CourseId,
                  CourseTitle = e.Course.Title,
                  Grade = e.Grade,
                  teacher = e.Course.Teacher,
                  classe = e.Course.Classe
              })
              .ToListAsync();


            return Ok(enrollments);
        }

        // POST api/enrollments

        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpPost]
        public async Task<IActionResult> AddEnrollment( EnrollmentCreateDto dto)
        {

            if (!await _dbcontext.Students.AnyAsync(s => s.Id == dto.StudentId))
            {
                return NotFound("Student not found.");
            }

            if (!await _dbcontext.Courses.AnyAsync(c => c.Id == dto.CourseId))
            {
                return NotFound("Course not found.");
            }

            bool alreadyEnrolled = await _dbcontext.enrollments
                .AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);

            if (alreadyEnrolled)
            {
                return Conflict("Student is already enrolled in this course.");
            }
            Enrollment enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                CreateAt = DateTime.UtcNow
            };
            DisplayOutpot? displayOutpot = await _dbcontext.enrollments.Include(e => e.Course)

                                         .Include(e => e.Student).Select(e => new DisplayOutpot
                                         {
                                             StudentId = dto.StudentId,
                                             CourseId = dto.CourseId,
                                             CourseTitile =e.Course.Title,
                                             StudentName=e.Student.FullName
            }).FirstOrDefaultAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);
        


            await _dbcontext.enrollments.AddAsync(enrollment);
            await   _dbcontext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnrollment),
                new { studentId = enrollment.StudentId, courseId = enrollment.CourseId },
                displayOutpot);
        }

     


        // DELETE api/enrollments/student/5/course/3
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpDelete("student/{studentId}/course/{courseId}")]
        public async Task<IActionResult> DeleteEnrollment(string studentId, int courseId)
        {
            Enrollment? enrollment = await _dbcontext.enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            _dbcontext.enrollments.Remove(enrollment);
            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }
    }
}

