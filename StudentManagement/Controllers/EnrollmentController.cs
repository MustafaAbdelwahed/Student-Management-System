using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DTO;
using StudentManagement.Model;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly MDBcontext _dbcontext;
        public EnrollmentController(MDBcontext mDBcontext)
        {
            _dbcontext = mDBcontext;
        }
    // GET api/enrollments
    [HttpGet]
    public IActionResult GetAllEnrollments()
        {
 
            List<EnrollmentGetDataDto> enrollments = _dbcontext.enrollments.Select(e => new EnrollmentGetDataDto
            {
                StudentId = e.StudentId,
                StudentName = e.Student.FullName,
                CourseId = e.CourseId,
                Grade = e.Grade,
                CourseTitle = e.Course.Title,
                classe=e.Course.classe,
                teacher=e.Course.teacher
            })
           
            .ToList();
          
            return Ok(enrollments);
        }

        // GET api/enrollments/student/5/course/3
        [HttpGet("student/{studentId}/course/{courseId}")]
        public IActionResult GetEnrollment(int studentId, int courseId)
        { 

            EnrollmentGetDataDto ? enrollment = _dbcontext.enrollments
         .Include(e => e.Course).Include(e => e.Student)
         .Select(e => new EnrollmentGetDataDto
         {
             StudentId = e.StudentId,
             StudentName = e.Student.FullName,
             CourseId = e.CourseId,
             CourseTitle = e.Course.Title,
             Grade = e.Grade,
             teacher = e.Course.teacher,
             classe = e.Course.classe
         })
         .FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
            {
                return NotFound();
            }

          

            return Ok(enrollment);
        }

        // GET api/enrollments/student/5  -> all courses of a student
        [HttpGet("student/{studentId}")]
        public IActionResult GetEnrollmentsByStudent(int studentId)
        {
            if (!_dbcontext.Students.Any(s => s.Id == studentId))
            {
                return NotFound("Student not found.");
            }

            List<EnrollmentGetDataDto> enrollments = _dbcontext.enrollments
                .Include(e => e.Course).Include(e => e.Student)
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentGetDataDto
                {
                    StudentId=  e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                  Grade=e.Grade,
                    teacher = e.Course.teacher,
                    classe = e.Course.classe
                })
                .ToList();

            return Ok(enrollments);
        }

        // GET api/enrollments/course/3  -> all students in a course
        [HttpGet("course/{courseId}")]
        public IActionResult GetEnrollmentsByCourse(int courseId)
        {
            if (!_dbcontext.Courses.Any(c => c.Id == courseId))
            {
                return NotFound("Course not found.");
            }

            List<EnrollmentGetDataDto> enrollments = _dbcontext.enrollments
              .Include(e => e.Course).Include(e => e.Student)
              .Where(e => e.CourseId == courseId)
              .Select(e => new EnrollmentGetDataDto
              {
                  StudentId = e.StudentId,
                  StudentName = e.Student.FullName,
                  CourseId = e.CourseId,
                  CourseTitle = e.Course.Title,
                  Grade=e.Grade,
                  teacher = e.Course.teacher,
                  classe = e.Course.classe
              })
              .ToList();
           

            return Ok(enrollments);
        }

        // POST api/enrollments
        [HttpPost]
        public IActionResult AddEnrollment(EnrollmentCreateDto dto)
        {

            if (!_dbcontext.Students.Any(s => s.Id == dto.StudentId))
            {
                return NotFound("Student not found.");
            }

            if (!_dbcontext.Courses.Any(c => c.Id == dto.CourseId))
            {
                return NotFound("Course not found.");
            }

            bool alreadyEnrolled = _dbcontext.enrollments
                .Any(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);

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

            _dbcontext.enrollments.Add(enrollment);
            _dbcontext.SaveChanges();

            return CreatedAtAction(
                nameof(GetEnrollment),
                new { studentId = enrollment.CourseId, courseId = enrollment.CourseId},
                enrollment);
        }

        // PUT api/enrollments/student/5/course/3  -> set the grade
        [HttpPut("student/{studentId}/course/{courseId}")]
        public IActionResult UpdateGrade(int studentId, int courseId, EnrollmentGradeDto dto)
        {
            Enrollment? enrollment = _dbcontext.enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            enrollment.Grade = dto.Grade;
            _dbcontext.SaveChanges();
            return NoContent();
        }

        // DELETE api/enrollments/student/5/course/3
        [HttpDelete("student/{studentId}/course/{courseId}")]
        public IActionResult DeleteEnrollment(int studentId, int courseId)
        {
            Enrollment? enrollment = _dbcontext.enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return NotFound();
            }

            _dbcontext.enrollments.Remove(enrollment);
            _dbcontext.SaveChanges();
            return NoContent();
        }
    }
}

