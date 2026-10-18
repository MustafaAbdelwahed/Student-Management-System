using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.DTO;
using StudentManagement.Model;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly MDBcontext _dbcontext;
        public CourseController(MDBcontext mDBcontext)
        {
            _dbcontext = mDBcontext;
        }

        [HttpGet]
        public IActionResult GetAllCourses()
        {
            List<Course> courses = _dbcontext.Courses.ToList();
            if (courses.Count == 0)
            {
                return NotFound();
            }
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public IActionResult GetCourseById(int id)
        {
            GetCourseDto? courseDto = _dbcontext.Courses
                .Where(c => c.Id == id)
                .Select(c => new GetCourseDto
                {
                    CourseId = c.Id,
                    CourseTitle = c.Title,
                    Description = c.Description,
                    Credits = c.Credits,
                    teacher = c.teacher,
                    classe = c.classe
                })
                .FirstOrDefault();
            //Course? course = _dbcontext.Courses
            //    .Include(c => c.Enrollments).ThenInclude(e=>e.Student)
            //    .FirstOrDefault(c => c.Id == id);

            if (courseDto == null)
            {
                return NotFound();
            }
            return Ok(courseDto);
        }

        [HttpPost]
        public IActionResult AddCourse(CreateCourseDto createCourseDto)
        {
            Course course = new Course
            {
                Id = createCourseDto.Id,
                Title = createCourseDto.Title,
                Description = createCourseDto.Description,
                Credits = createCourseDto.Credits,
                teacher = createCourseDto.teacher,
                 classe= createCourseDto.classe
            };
            _dbcontext.Courses.Add(course);
            _dbcontext.SaveChanges();
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCourse(int id, UpdateCourseDto updatedCourse)
        {
            Course? course = _dbcontext.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            course.Title = updatedCourse.Title;
            course.Description = updatedCourse.Description;
            course.Credits = updatedCourse.Credits;
            course.teacher = updatedCourse.teacher;
            course.classe = updatedCourse.classe;

            _dbcontext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            Course? course = _dbcontext.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            _dbcontext.Courses.Remove(course);
            _dbcontext.SaveChanges();
            return NoContent();
        }
    }
}
