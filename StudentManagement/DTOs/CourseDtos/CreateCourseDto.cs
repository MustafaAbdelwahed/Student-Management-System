using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.CreateCourseDto
{
    public record CourseDto(string Id, string Title, string? Description, int Credits);

    public class CreateCourseDto
    {

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;


        [StringLength(200)]
        public string teacher { get; set; } = string.Empty;

        [StringLength(200)]
        public string classe { get; set; } = string.Empty;
    }
}
