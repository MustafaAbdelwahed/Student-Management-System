using Microsoft.AspNetCore.Http.HttpResults;
using StudentManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO
{

    public record StudentDto(int Id, string FullName, string Email, DateTime DateOfBirth);

    public class CreateStudentDto
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;





    }

    public class UpdateStudentDto
    {

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Password { get; set; } = string.Empty;

        [Required ]
        public DateOnly DateOfBirth { get; set; }
    }

}
