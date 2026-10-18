using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Model
{
    public class MDBcontext:DbContext
    {
      
        public MDBcontext(DbContextOptions<MDBcontext> option):base(option)
        {
            
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> enrollments { get; set; }
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Student>(e =>
            {
                e.Property(x => x.FullName).IsRequired().HasMaxLength(150);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.HasIndex(x => x.Email).IsUnique();
            });

            b.Entity<Course>(e =>
            {
                e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            });

            b.Entity<Enrollment>(e =>
            {
                e.HasKey(x => new { x.StudentId, x.CourseId }); // composite key
                e.Property(x => x.Grade).HasPrecision(5, 2);

                e.HasOne(x => x.Student)
                 .WithMany(s => s.Enrollments)
                 .HasForeignKey(x => x.StudentId);

                e.HasOne(x => x.Course)
                 .WithMany(c => c.Enrollments)
                 .HasForeignKey(x => x.CourseId);
            });
        }
        //public DbSet<SchoolClass> schoolClasses { get; set; }
        //public DbSet<Admin> admins { get; set; }
        //public DbSet<Teacher> teachers { get; set; }


    }

}
