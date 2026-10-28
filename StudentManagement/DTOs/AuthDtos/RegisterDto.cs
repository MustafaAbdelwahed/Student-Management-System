using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.UserCreatedDto
{
    public record CreateUserDtos(string Id, string FullName, string Email, string Role);

    public enum UserRole { Student, Teacher, Admin }

        public class RegisterDto
        {

            [Required, MaxLength(100)]
            public string FullName { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, MinLength(8)]
            public string Password { get; set; } = string.Empty;
            [Required]
            public string? Role { get; set; }
        }
    
}
