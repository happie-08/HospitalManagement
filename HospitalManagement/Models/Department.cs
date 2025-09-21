using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        public string? Description { get; set; }

        public bool Active { get; set; }
        public ICollection<ReferenceDoctor> ReferenceDoctors { get; set; }

    }
}
