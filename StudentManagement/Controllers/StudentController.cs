
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DTO;
using StudentManagement.Model;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly MDBcontext _dbcontext;
        public StudentController(MDBcontext mDBcontext)
        {
            _dbcontext = mDBcontext;
        }

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            List<Student> students = _dbcontext.Students.ToList();
            if (students.Count == 0)
            {
                return NotFound("Ther is no Student");
            }
            return Ok(students);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetStudentById(int id)
        {
            Student? student = _dbcontext.Students
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound("Ther is no Student");
            }
            return Ok(student);
        }



        [HttpPost]
        public IActionResult AddStudent(CreateStudentDto createStudentDto)
        {
            Student student = new Student
            {
                Id = createStudentDto.Id,
                FullName = createStudentDto.FullName,
                Email = createStudentDto.Email,
                Password = createStudentDto.Password,
                DateOfBirth = createStudentDto.DateOfBirth,
                CreatedAt = createStudentDto.CreatedAt
            };
            _dbcontext.Students.Add(student);
            _dbcontext.SaveChanges();
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, UpdateStudentDto updatedStudent)
        {
            Student? student = _dbcontext.Students.Find(id);
            if (student == null)
            {
                return NotFound("Ther is no Student whit this id");
            }
            UpdateStudentDto updateStudent = new UpdateStudentDto();

            student.FullName = updatedStudent.FullName;
            student.Email = updatedStudent.Email;
            student.DateOfBirth = updatedStudent.DateOfBirth;


            _dbcontext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            Student? student = _dbcontext.Students.Find(id);
            if (student == null)
            {
                return NotFound("Ther is no Student whit this id");
            }

            _dbcontext.Students.Remove(student);
            _dbcontext.SaveChanges();
            return NoContent();
        }

        //// POST api/students/5/courses/3
        //[HttpPost("{studentId}/courses/{courseId}")]
        //public IActionResult Enroll(int studentId, int courseId)
        //{
        //    bool studentExists = _dbcontext.Students.Any(s => s.Id == studentId);
        //    bool courseExists = _dbcontext.Courses.Any(c => c.Id == courseId);
        //    if (!studentExists || !courseExists)
        //    {
        //        return NotFound();
        //    }

        //    bool alreadyEnrolled = _dbcontext.enrollments
        //        .Any(e => e.StudentId == studentId && e.CourseId == courseId);
        //    if (alreadyEnrolled)
        //    {
        //        return Conflict("Student is already enrolled in this course.");
        //    }

        //    _dbcontext.enrollments.Add(new Enrollment { StudentId = studentId, CourseId = courseId });
        //    _dbcontext.SaveChanges();
        //    return NoContent();
        //}

        // DELETE api/students/5/courses/3
        //    [HttpDelete("{studentId}/courses/{courseId}")]
        //    public IActionResult Unenroll(int studentId, int courseId)
        //    {
        //        Enrollment? enrollment = _dbcontext.enrollments
        //            .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

        //        if (enrollment == null)
        //        {
        //            return NotFound();
        //        }

        //        _dbcontext.enrollments.Remove(enrollment);
        //        _dbcontext.SaveChanges();
        //        return NoContent();
        //    }
        //}
    }
}

