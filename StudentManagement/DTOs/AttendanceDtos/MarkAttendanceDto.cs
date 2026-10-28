using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs.AttendanceDtos
{
    public class MarkAttendanceDto
    {
        public DateOnly Date { get; set; }

        [MinLength(1)]
        public List<AttendanceEntryDto> Records { get; set; } = new();
    }
}
