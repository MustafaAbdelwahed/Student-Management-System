
using CompanySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StudentManagement.Data;
using StudentManagement.DTO.GetAllStudentDto;
using StudentManagement.DTO.GetStudentByIdDto;
using StudentManagement.DTO.UpdateStudentDto;
using StudentManagement.Model;
using System.Security.Claims;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _dbcontext;
        private readonly UserManager<AppUser> _userManager;
        private bool IsAdmin => User.IsInRole(Roles.Admin);
        public StudentController(AppDbContext mDBcontext, UserManager<AppUser> userManager)
        {
            _dbcontext = mDBcontext;
            _userManager = userManager;
        }

        //Get All Student
        [Authorize (Roles = $"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("Get All Students")]
        public async Task<IActionResult> GetAllStudents()
        {
            List<GetAllStudentDto> result = await _dbcontext.Students
                .OrderBy(s => s.FullName)
                .Select(s => new GetAllStudentDto
                {
                    UserId = s.Id,
                    FullName = s.FullName,
                    Email = s.Email ?? string.Empty,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
            if (result.Count==0)
            {
                return BadRequest("Ther is no Student");
            }
            return Ok(result); 
        }

        //Get Student By ID
        [Authorize(Roles =$"{Roles.Admin},{Roles.Teacher}")]
        [HttpGet("getStudentByID/{id}")]
        public async Task<IActionResult> GetStudentsById( string id)
        {
            Student? student = await  _dbcontext.Students
              .AsNoTracking()
              .Include(s => s.Enrollments)
              .ThenInclude(e => e.Course)
              .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return BadRequest("Ther is no Student with this ID ");
            }
            GetStudentByIdDto result = new GetStudentByIdDto
            {
                UserId = student.Id,
                FullName = student.FullName,
                Email = student.Email ,
                CreatedAt=student.CreatedAt,
                courses = student.Enrollments.Select(e => e.Course.Title).ToList() ?? new List<string>()
            };
            return Ok(result);
        }

        [Authorize(Roles = $"{Roles.Student}")]
        [HttpGet("GetMyData")]
        public async Task<IActionResult> GetStudentData()
        {
            string? studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            Student? student = await _dbcontext.Students
              .AsNoTracking()
              .Include(s => s.Enrollments)
              .ThenInclude(e => e.Course)
              .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return BadRequest("Ther is no Student with this ID ");
            }
            GetStudentByIdDto result = new GetStudentByIdDto
            {
                UserId = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                CreatedAt = student.CreatedAt,
                courses = student.Enrollments.Select(e => e.Course.Title).ToList() ?? new List<string>()
            };
            return Ok(result);
        }

        // Admin: any student. Student: only their own.
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditStudent(string id, EditStudentDto dto)
        {

            if (!IsAdmin && id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            Student? student = await _dbcontext.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student is null)
                return NotFound("There is no Student with this id");

            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound("LoginUser account not found");

            // the new email must not belong to someone else
            AppUser? emailOwner = await _userManager.FindByEmailAsync(dto.Email);

            if (emailOwner is not null && emailOwner.Id != id)
                return Conflict(new { message = "This email is already used by another account." });

            await using var tx = await _dbcontext.Database.BeginTransactionAsync();

            // 1. login account (we use the email as the username, so update both)
            user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                 await _userManager.SetEmailAsync(user, dto.Email);
                 await _userManager.SetUserNameAsync(user, dto.Email);
                 student.Email = dto.Email;
            }
            else
            await _userManager.UpdateAsync(user);

            student.FullName = string.IsNullOrWhiteSpace(dto.FullName) ? student.FullName: dto.FullName;
            await _dbcontext.SaveChangesAsync();
            await tx.CommitAsync();
            //return Ok(dto);
            return NoContent();
        }

        //Delete Student
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            if (!IsAdmin && id != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            Student? student = _dbcontext.Students.Find(id);
            if (student == null)
                return NotFound(new { message = "Ther is no Student whit this id" });

            AppUser? user =await _userManager.FindByIdAsync(id);

            if (user != null&& !await _userManager.IsInRoleAsync(user, Roles.Admin)&& await _userManager.IsInRoleAsync(user, Roles.Teacher))
                return BadRequest(new { message = "  (Admin or Teacher)  account can't be deleted here." });

            await using var tx = await _dbcontext.Database.BeginTransactionAsync();

            _dbcontext.Students.Remove(student);
            await _dbcontext.SaveChangesAsync();
            if (user!=null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));
            }
            await tx.CommitAsync();
            return NoContent();
        }
   
    }
}

