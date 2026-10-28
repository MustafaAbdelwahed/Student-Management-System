using StudentManagement.Model;

namespace StudentManagement.DTO.GetStudentByIdDto
{
    public class GetStudentByIdDto
    {

        public string? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string HasProfile { get; set; } = string.Empty;

        public List<string> courses { get; set; } = new();


        public DateTime CreatedAt { get; set; }
    }
}
