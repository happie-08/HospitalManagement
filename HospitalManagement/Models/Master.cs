using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Master
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; } = true;


    }
}
