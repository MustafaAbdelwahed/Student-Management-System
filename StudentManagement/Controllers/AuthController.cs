using CompanySystem.DTOs;
using CompanySystem.DTOs.LoginDto;
using CompanySystem.Models;
using CompanySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.DTO.AssignRoleDto;
using StudentManagement.DTO.AuthDtos;
using StudentManagement.DTO.CreateStudentDto;
using StudentManagement.DTO.UserCreatedDto;
using StudentManagement.DTOs.AuthDtos;
using StudentManagement.Model;
using System.Security.Claims;

namespace CompanySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbcontext;
        private readonly ITokenService _tokenService;
        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService,AppDbContext appDbContext)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _dbcontext = appDbContext;
        }
      

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<ActionResult<CreateUserDtos>> CreateUser(RegisterDto dto)
        {
             if (!Enum.IsDefined(typeof(UserRole), dto.Role!))
                 return BadRequest("Role must be Student, Teacher or Admin.");

            AppUser user = new AppUser
             {
                 UserName = dto.Email,
                 Email = dto.Email,
                 FullName = dto.FullName
             };

            var result = await _userManager.CreateAsync(user, dto.Password);
             if (!result.Succeeded) return BadRequest(result.Errors);
          
             string roleName = dto.Role!.ToString();
             await _userManager.AddToRoleAsync(user, roleName);

            if  (dto.Role== "Student")
            {
                Student student = new Student
                {
                    Id = user.Id,
                    FullName = dto.FullName,
                    Email = dto.Email,
                };
               await _dbcontext.Students.AddAsync(student);
             await   _dbcontext.SaveChangesAsync();

            }

            return CreatedAtAction(nameof(CreateUser), new { id = user.Id },
            await BuildResponse(user));
 
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginUser(LoginDto dto)
        {
            AppUser? user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Unauthorized(new { message = "Invalid email or password." });

            if (await _userManager.IsLockedOutAsync(user))
                return StatusCode(423, new { message = "Account locked. Try again later." });

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            //await _userManager.ResetAccessFailedCountAsync(user);
            return Ok(await BuildResponse(user));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<GetUserDto>> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            AppUser? user = userId is null ? null : await _userManager.FindByIdAsync(userId);
            if (user is null) 
                return Unauthorized();
            GetUserDto getUserDto = new GetUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No Role"
            };

            return Ok(getUserDto);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> EditRole(string id, AssignRoleDto roleDto)
        {
            if (id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                return BadRequest(new { message = "You can't edit your own account." });


            if (!Roles.All.Contains(roleDto.Role))
                return BadRequest(new { message = $"Role must be one of: {string.Join(", ", Roles.All)}" });

      

            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound("User not found.");
            
            var current =  ( await _userManager.GetRolesAsync(user));
            foreach (var item in current)
            {
                if (item == "Student")
                {
                    Student? student =await _dbcontext.Students.FindAsync(id);
                    _dbcontext.Students.Remove(student);
                    await _dbcontext.SaveChangesAsync();
                    break;
                }
            }
            if (roleDto.Role=="Student")
            {
                Student student = new Student
                {
                    
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    CreatedAt=user.CreatedAt,

                };
                await _dbcontext.Students.AddAsync(student);
                await _dbcontext.SaveChangesAsync();
            }


            await _userManager.RemoveFromRolesAsync(user, current);
            await _userManager.AddToRoleAsync(user, roleDto.Role);

            return Ok(new { user.Id, user.Email, Role = roleDto.Role });
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("users")]
        public async Task<ActionResult<List<GetUserDto>>> GetUsers()
        {
            List<AppUser> users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .ToListAsync();

            List<GetUserDto> result = new List<GetUserDto>();
            foreach (AppUser User in users)
            {
                string role = (await _userManager.GetRolesAsync(User)).FirstOrDefault() ?? "No Role";
                result.Add(new GetUserDto
                {
                  Id=  User.Id,
                    FullName= User.FullName,
                    Email= User.Email,
                    Role = role
                });
            }
            return Ok(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // an Admin can't delete themselves by accident
            if (id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                return BadRequest(new { message = "You can't delete your own account." });

            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound(new { message = "User not found." });

            await using var tx = await _dbcontext.Database.BeginTransactionAsync();

            // 1. the student profile, if there is one (its enrollments are removed with it)
            Student? student = await _dbcontext.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student is not null)
            {
                _dbcontext.Students.Remove(student);
                await _dbcontext.SaveChangesAsync();
            }

            // 2. the login account
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            await tx.CommitAsync();

            return NoContent();
        }

        //// Test endpoints for role-based access
        //[Authorize(Roles = Roles.Admin)]
        //[HttpGet("admin-only")]
        //public IActionResult AdminOnly() => Ok("Hello Admin");

        //[Authorize(Roles = $"{Roles.Teacher},{Roles.Student}")]
        //[HttpGet("Std-or-Teacher")]
        //public IActionResult CsOrIt() => Ok("Hello Student or Teacher");

        //Build Token
        private async Task<AuthResponseDto> BuildResponse(AppUser user)
        {
            var roles = await  _userManager.GetRolesAsync(user);
            var (token, expires) = _tokenService.CreateToken(user, roles);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expires,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Roles = roles
            };
        }
        
    }
}
