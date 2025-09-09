namespace HospitalManagement.ViewModels
{
    public class OPDInvoiceViewModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DiagnosisName { get; set; }
        public string SymptomName { get; set; }
        public decimal Amount { get; set; }
    }
}
