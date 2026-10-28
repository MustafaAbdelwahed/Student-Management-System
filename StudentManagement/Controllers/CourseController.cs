using CompanySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.DTO.CreateCourseDto;
using StudentManagement.DTO.GetCourseDto;
using StudentManagement.DTO.UpdateCourseDto;
using StudentManagement.Model;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        public CourseController(AppDbContext mDBcontext)
        {
            _dbcontext = mDBcontext;
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            List<Course> courses =await _dbcontext.Courses.ToListAsync();
            if (courses.Count == 0)
            {
                return NotFound();
            }
            List<GetCourseDto> getCourseDtos =await _dbcontext.Courses.Select(c => new GetCourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                teacher = c.Teacher,
                classe = c.Classe
            }).ToListAsync();

            return Ok(courses);
        }
        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            GetCourseDto? courseDto =await _dbcontext.Courses
                .Where(c => c.Id == id)
                .Select(c => new GetCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    teacher = c.Teacher,
                    classe = c.Classe
                })
                .FirstOrDefaultAsync();
       
            if (courseDto == null)
            {
                return NotFound();
            }
            return Ok(courseDto);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpPost]
        public async Task<IActionResult> AddCourse(CreateCourseDto createCourseDto)
        {
            Course course = new Course
            {
                Title = createCourseDto.Title,
                Description = createCourseDto.Description,
                Teacher = createCourseDto.teacher,
                Classe = createCourseDto.classe
            };
            await _dbcontext.Courses.AddAsync(course);
            await _dbcontext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, createCourseDto);
        }

        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseDto updatedCourse)
        {
            Course? course =await _dbcontext.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrWhiteSpace(updatedCourse.Title))
                course.Title = updatedCourse.Title;

            if (!string.IsNullOrWhiteSpace(updatedCourse.Description))
                course.Description = updatedCourse.Description;

            if (!string.IsNullOrWhiteSpace(updatedCourse.teacher))
                course.Teacher = updatedCourse.teacher;

            if (!string.IsNullOrWhiteSpace(updatedCourse.classe))
                course.Classe = updatedCourse.classe;

            await  _dbcontext.SaveChangesAsync();
            return Ok(course);
            //return NoContent();
        }

        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            Course? course =await _dbcontext.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            _dbcontext.Courses.Remove(course);
            await _dbcontext.SaveChangesAsync();
            return NoContent();
        }
    }
}
