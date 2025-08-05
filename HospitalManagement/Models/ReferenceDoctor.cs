using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class ReferenceDoctor
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Hospital Name")]
        public string HospitalName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }

        public string Degree { get; set; }

        [Phone]
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public bool Active { get; set; }
    }
}
