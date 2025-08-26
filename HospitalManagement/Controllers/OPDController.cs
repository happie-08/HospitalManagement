using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class OPDController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OPDController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================== LIST PAGE ==================
        public IActionResult Index()
        {
            return View();
        }

        // ================== LOAD DROPDOWNS ==================
        private void LoadDropdowns(OPD opd = null)
        {
            ViewBag.Patients = new SelectList(
                _context.Patients
                    .Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName })
                    .ToList(),
                "PatientId",
                "FullName",
                opd?.PatientId // pre-select value for Edit
            );

            ViewBag.Doctors = new SelectList(
                _context.ReferenceDoctors
                    .Select(d => new { d.Id, FullName = d.FirstName + " " + d.LastName })
                    .ToList(),
                "Id",
                "FullName",
                opd?.DoctorId
            );

            ViewBag.DiagnosisList = new SelectList(
                _context.Masters.Where(m => m.Type == "Diagnosis").ToList(),
                "Id",
                "Name",
                opd?.DiagnosisId
            );

            ViewBag.SymptomList = new SelectList(
                _context.Masters.Where(m => m.Type == "Symptoms").ToList(),
                "Id",
                "Name",
                opd?.SymptomId
            );
        }

        // ================== CREATE (GET) ==================
        [HttpGet]
        public IActionResult Create()
        {
            var lastOpd = _context.OPDs
                                  .OrderByDescending(o => o.Id)
                                  .FirstOrDefault();

            int nextNumber = 1;
            if (lastOpd != null && !string.IsNullOrEmpty(lastOpd.InvoiceNumber))
            {
                var numberPart = lastOpd.InvoiceNumber.Replace("NP", "");
                if (int.TryParse(numberPart, out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            var opd = new OPD
            {
                InvoiceNumber = "NP" + nextNumber,
                PaymentDate = DateTime.Today
            };

            LoadDropdowns();
            return View(opd);
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OPD opd)
        {
            // Debug ModelState errors
            if (ModelState.IsValid)
            {
                var errors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();

                // Reload dropdowns so form works again
                LoadDropdowns();
                ViewBag.IsEdit = false;

                // Optionally show errors in view (or return JSON for debugging)
                ViewBag.Errors = errors;
                return View(opd);
            }

            // Auto-generate InvoiceNumber
            var lastOpd = _context.OPDs.OrderByDescending(o => o.Id).FirstOrDefault();
            int nextNumber = 1;
            if (lastOpd != null && !string.IsNullOrEmpty(lastOpd.InvoiceNumber))
            {
                var numberPart = lastOpd.InvoiceNumber.Replace("NP", "");
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            opd.InvoiceNumber = "NP" + nextNumber;

            // Set default PaymentDate if null
            if (opd.PaymentDate == default) opd.PaymentDate = DateTime.Today;

            // Insert into DB
            _context.OPDs.Add(opd);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // ================== EDIT (GET) ==================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var opd = _context.OPDs.Find(id);
            if (opd == null)
                return NotFound();

            LoadDropdowns(opd);
            ViewBag.IsEdit = true;
            return View("Create", opd);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OPD opd)
        {
            if (ModelState.IsValid)
            {
                LoadDropdowns(opd);
                ViewBag.IsEdit = true;
                return View("Create", opd);
            }

            var existingOpd = await _context.OPDs.FindAsync(opd.Id);
            if (existingOpd == null)
                return NotFound();

            // Update only necessary fields
            existingOpd.PatientId = opd.PatientId;
            existingOpd.DoctorId = opd.DoctorId;
            existingOpd.DiagnosisId = opd.DiagnosisId;
            existingOpd.SymptomId = opd.SymptomId;
            existingOpd.Amount = opd.Amount;
            existingOpd.PaymentDate = opd.PaymentDate;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE ==================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var opd = await _context.OPDs.FindAsync(id);
            if (opd == null)
                return Json(new { success = false, message = "Record not found." });

            _context.OPDs.Remove(opd);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Record deleted successfully." });
        }

        // ================== GET ALL (for DataTables Ajax) ==================
        [HttpGet]
        public IActionResult GetAll()
        {
            var opdList = _context.OPDs
                .Include(o => o.Patient)
                .Include(o => o.Doctor)
                .Include(o => o.Diagnosis)
                .Include(o => o.Symptom)
                .Select(o => new
                {
                    id = o.Id,
                    patientName = o.Patient != null ? (o.Patient.FirstName + " " + o.Patient.LastName).Trim() : "",
                    doctorName = o.Doctor != null ? (o.Doctor.FirstName + " " + o.Doctor.LastName).Trim() : "",
                    diagnosisName = o.Diagnosis != null ? o.Diagnosis.Name : "",
                    symptomName = o.Symptom != null ? o.Symptom.Name : "",
                    amount = o.Amount,
                    paymentDate = o.PaymentDate.ToString("yyyy-MM-dd"),
                    invoiceNumber = o.InvoiceNumber
                })
                .ToList();

            return Json(new { data = opdList });
        }
    }
}
