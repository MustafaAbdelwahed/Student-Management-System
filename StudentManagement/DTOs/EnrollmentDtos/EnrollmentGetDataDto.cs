using StudentManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.EnrollmentGetDataDto
{
 
    public class EnrollmentGetDataDto
    {
        public string? StudentId { get; set; }

        [Required, StringLength(150)]
        public string StudentName { get; set; } = string.Empty;

        public int CourseId { get; set; }

        [Required, StringLength(200)]
        public string CourseTitle { get; set; } = string.Empty;

      

        [StringLength(200)]
        public string teacher { get; set; } = string.Empty;

        [StringLength(200)]
        public string classe { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal? Grade { get; set; }
    }

 


   
}
