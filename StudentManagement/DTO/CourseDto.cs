using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO
{
    public record CourseDto(int Id, string Title, string? Description, int Credits);

    public class CreateCourseDto
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Credits { get; set; } 

        [ StringLength(200)]
        public string teacher { get; set; } = string.Empty;

        [ StringLength(200)]
        public string classe { get; set; } = string.Empty;
    }
    public class GetCourseDto
    {
        public int StudentId { get; set; }

        [StringLength(150)]
        public string StudentName { get; set; } = string.Empty;

        public int CourseId { get; set; }

        [ StringLength(200)]
        public string CourseTitle { get; set; } = string.Empty;

        public string? Description { get; set; }
        public int Credits { get; set; }
        public string teacher { get; set; } = string.Empty;
        public string classe { get; set; } = string.Empty;

    }


    public class UpdateCourseDto 
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Credits { get; set; }

        [StringLength(200)]
        public string teacher { get; set; } = string.Empty;

        [StringLength(200)]
        public string classe { get; set; } = string.Empty;
    }

}

