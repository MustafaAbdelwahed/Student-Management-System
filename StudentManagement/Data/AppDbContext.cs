using CompanySystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Model;

namespace StudentManagement.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
      
        public AppDbContext(DbContextOptions<AppDbContext> option):base(option)
        {
            
        }
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
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
            b.Entity<Attendance>()
                .HasIndex(a => new { a.StudentId, a.CourseId, a.Date })
                .IsUnique();                       // one record per student, per course, per day

            b.Entity<Attendance>()
                .Property(a => a.Status)
                .HasConversion<string>();          // stored as "Present", "Absent" ... instead of 0, 1
        }



    }

}
