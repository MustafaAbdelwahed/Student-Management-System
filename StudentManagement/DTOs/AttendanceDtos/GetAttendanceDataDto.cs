using StudentManagement.Model;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs.AttendanceDtos
{

    public class GetAttendanceDataDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
