namespace StudentManagement.Model
{
    public class Enrollment
    {

        public string StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; } 
        public Course Course { get; set; } = null!;
        public DateTime CreateAt { get; set; }= DateTime.Now;
        public decimal? Grade { get; set; }
    }
}
