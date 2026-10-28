using System.ComponentModel.DataAnnotations;

namespace StudentManagement.DTO.AssignRoleDto
{
    public class AssignRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
