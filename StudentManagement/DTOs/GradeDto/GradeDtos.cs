using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs.GradeDto
{
    public class SetGradeDto
    {
        [Range(0, 100)]
        public decimal Grade { get; set; }
    }

    public class GetGradeDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public decimal? Grade { get; set; }
    }
    public class GetGradeForStdDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public decimal? Grade { get; set; }
    }

}
