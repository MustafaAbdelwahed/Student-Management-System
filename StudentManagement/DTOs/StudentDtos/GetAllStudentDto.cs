namespace StudentManagement.DTO.GetAllStudentDto
{
    public class GetAllStudentDto
    {
        public string? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HasProfile { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
