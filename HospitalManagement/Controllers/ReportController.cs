using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    [Route("Report")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== OPD Report Page =====================
        [HttpGet("OPDReport")]
        public IActionResult OPDReport()
        {
            return View();
        }

        // ===================== Get Filters for Dropdowns =====================
        [HttpGet("GetOPDFilters")]
        public IActionResult GetOPDFilters()
        {
            var doctors = _context.ReferenceDoctors.Where(d => d.Active)
                .Select(d => new
                {
                    id = d.Id,
                    name = d.FirstName + " " + d.LastName
                }).ToList();

            var patients = _context.Patients
                .Select(p => new
                {
                    id = p.PatientId,
                    fullName = p.FirstName + " " + p.LastName
                }).ToList();

            var paymentDates = _context.OPDs
                .Select(o => o.PaymentDate.ToString("dd-MM-yyyy"))

                .Distinct()
                .ToList();

            return Json(new { doctors, patients, paymentDates });
        }

        // ===================== Get OPD Report Data =====================
        [HttpGet("GetOPDReportData")]
        public IActionResult GetOPDReportData(int? doctorId, int? patientId, string dateRange)
        {
            var query = _context.OPDs
                .Include(o => o.Doctor)
                .Include(o => o.Patient)
                .AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(o => o.DoctorId == doctorId);

            if (patientId.HasValue)
                query = query.Where(o => o.PatientId == patientId);

            if (!string.IsNullOrEmpty(dateRange))
            {
                var dates = dateRange.Split(" - ");
                if (dates.Length == 2)
                {
                    DateTime startDate = DateTime.Parse(dates[0]);
                    DateTime endDate = DateTime.Parse(dates[1]).AddDays(1).AddTicks(-1);
                    query = query.Where(o => o.PaymentDate >= startDate && o.PaymentDate <= endDate);
                }
            }

            var data = query.Select(o => new
            {
                patientName = o.Patient.FirstName + " " + o.Patient.LastName,
                doctorName = o.Doctor.FirstName + " " + o.Doctor.LastName,
                paymentDate = o.PaymentDate.ToString("dd-MM-yyyy"),
                amount = o.Amount,
                invoiceNumber = o.InvoiceNumber
            }).ToList();

            return Json(new { data });
        }

    }
}
