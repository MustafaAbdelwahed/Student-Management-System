using StudentManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs.AttendanceDtos
{
    public class AttendanceEntryDto
    {
        [Required]
        public string StudentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
