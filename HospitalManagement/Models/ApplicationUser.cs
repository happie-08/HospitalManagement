using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string? Name { get; set; }
        public string? Hobby { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
        [Display(Name = "Role")]
        public int? RoleId { get; set; } // FK column
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
