namespace StudentManagement.Model
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late,
        Excused
    }

    public class Attendance
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; } 
        public Course Course { get; set; } = null!;

        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
