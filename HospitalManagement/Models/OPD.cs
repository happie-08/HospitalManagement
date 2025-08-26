using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace HospitalManagement.Models
{
    public class OPD
    {
        [Key]
        public int Id { get; set; }

        // Foreign key: Patient
        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public ReferenceDoctor Doctor { get; set; }

        [Display(Name = "Diagnosis")]
        public int? DiagnosisId { get; set; }
        [ForeignKey("DiagnosisId")]
        public Master Diagnosis { get; set; }

        [Display(Name = "Symptom")]
        public int? SymptomId { get; set; }
        [ForeignKey("SymptomId")]
        public Master Symptom { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [MaxLength(50)]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

    }
}
