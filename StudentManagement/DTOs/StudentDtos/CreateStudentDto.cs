using Microsoft.AspNetCore.Http.HttpResults;
using StudentManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.CreateStudentDto
{

    public record StudentDto(string Id, string FullName, string Email, DateTime DateOfBirth);

    public class CreateStudentDto
    {
        public string? Id { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;





    }





}
