using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool Active { get; set; } = true;

        // 🔥 Navigation to Users
        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
