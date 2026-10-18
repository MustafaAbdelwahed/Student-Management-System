namespace StudentManagement.Model
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Credits { get; set; }
        public string teacher { get; set; } = string.Empty;
        public string classe { get; set; } = string.Empty;


        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        //public int Id { get; set; }
        //public string Name { get; set; } = string.Empty;
        //public string Description { get; set; }
        //public int StudentId { get; set; }
        ////public ICollection<Enrollment> enrollments { get; set; } = new List<Enrollment>();


    }
}
