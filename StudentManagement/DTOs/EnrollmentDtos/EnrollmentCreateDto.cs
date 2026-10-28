using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.EnrollmentCreateDto
{
    public class EnrollmentCreateDto
    {
        [Required]
        public string? StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        //public string StudentName { get; set; } = string.Empty;

        //public string CourseTitle { get; set; } = string.Empty;

    }

    public class DisplayOutpot
    {
        public string? StudentId { get; set; }

        public string? StudentName { get; set; }
        public string? CourseTitile { get; set; }

        public int CourseId { get; set; }
 
    }
}

