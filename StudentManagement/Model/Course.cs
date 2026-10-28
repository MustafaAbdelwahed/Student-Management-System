namespace StudentManagement.Model
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public string Classe { get; set; } = string.Empty;


        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
   


    }
}
