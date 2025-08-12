using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        public float? Height { get; set; }

        public float? Weight { get; set; }
        [Phone]
        [Display(Name = "Contact No")]
        public string? ContactNo { get; set; }

        public string? Image { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(500)] // Optional: limit the length
        public string? Remark { get; set; }


        // Foreign Key
        public int? ReferenceDoctorId { get; set; }

        // Navigation Property
        public ReferenceDoctor? ReferenceDoctor { get; set; }
    }

}
