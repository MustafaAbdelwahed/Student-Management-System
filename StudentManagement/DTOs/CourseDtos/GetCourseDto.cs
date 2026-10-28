using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.GetCourseDto
{
   
    public class GetCourseDto
    {

        public int? Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string teacher { get; set; } = string.Empty;
        public string classe { get; set; } = string.Empty;

    }



}

