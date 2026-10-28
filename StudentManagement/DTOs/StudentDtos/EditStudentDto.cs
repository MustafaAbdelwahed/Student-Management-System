using System.ComponentModel.DataAnnotations;
using System.Data;

namespace StudentManagement.DTO.UpdateStudentDto
{
    public class EditStudentDto
    {

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;



    }
}
