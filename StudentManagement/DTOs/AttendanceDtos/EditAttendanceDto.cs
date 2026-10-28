using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTOs.AttendanceDtos
{
    public class EditAttendanceDto
    {
        [Required]
      public DateOnly date { get; set; }

        [Required]
        public string? status { get; set; }
    }
}
