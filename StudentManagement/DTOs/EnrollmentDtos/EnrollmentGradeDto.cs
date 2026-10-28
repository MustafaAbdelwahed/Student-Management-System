using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.EnrollmentDtos
{
    public class EnrollmentGradeDto
    {
        [Range(0, 100)]
        public decimal? Grade { get; set; }
    }
}
