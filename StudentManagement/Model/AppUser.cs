using Microsoft.AspNetCore.Identity;

namespace CompanySystem.Models
{

    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public static class Roles
    {
        
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";

        public static readonly string[] All = { Admin, Teacher, Student };
    }
}
