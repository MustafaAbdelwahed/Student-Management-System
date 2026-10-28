using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.UpdateCourseDto
{

    public class UpdateCourseDto
    {
        [ StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;



        [StringLength(200)]
        public string teacher { get; set; } = string.Empty;

        [StringLength(200)]
        public string classe { get; set; } = string.Empty;
    }
}
