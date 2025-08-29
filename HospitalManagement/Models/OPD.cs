using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // ✅ Many-to-Many for Diagnosis
        public List<OPDDiagnosis> OPDDiagnoses { get; set; } = new List<OPDDiagnosis>();

        // ✅ Many-to-Many for Symptom
        public List<OPDSymptom> OPDSymptoms { get; set; } = new List<OPDSymptom>();

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

    public class OPDDiagnosis
    {
        public int OPDId { get; set; }
        public OPD OPD { get; set; }

        public int DiagnosisId { get; set; }
        public Master Diagnosis { get; set; }
    }

    public class OPDSymptom
    {
        public int OPDId { get; set; }
        public OPD OPD { get; set; }

        public int SymptomId { get; set; }
        public Master Symptom { get; set; }
    }
}
