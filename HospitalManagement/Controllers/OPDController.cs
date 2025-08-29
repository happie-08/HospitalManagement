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
        private async Task LoadDropdowns(OPD opd = null)
        {
            ViewBag.Patients = new SelectList(
                await _context.Patients
                    .Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName })
                    .ToListAsync(),
                "PatientId",
                "FullName",
                opd?.PatientId
            );

            ViewBag.Doctors = new SelectList(
                await _context.ReferenceDoctors
                    .Select(d => new { d.Id, FullName = d.FirstName + " " + d.LastName })
                    .ToListAsync(),
                "Id",
                "FullName",
                opd?.DoctorId
            );

            ViewBag.DiagnosisList = new MultiSelectList(
                await _context.Masters.Where(m => m.Type == "Diagnosis").ToListAsync(),
                "Id",
                "Name",
                opd?.OPDDiagnoses?.Select(d => d.DiagnosisId)
            );

            ViewBag.SymptomList = new MultiSelectList(
                await _context.Masters.Where(m => m.Type == "Symptoms").ToListAsync(),
                "Id",
                "Name",
                opd?.OPDSymptoms?.Select(s => s.SymptomId)
            );
        }

        // ================== CREATE (GET) ==================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var opd = new OPD
            {
                InvoiceNumber = await GenerateInvoiceNumber(),
                PaymentDate = DateTime.Today
            };

            await LoadDropdowns();
            return View(opd);
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OPD opd, int[] DiagnosisId, int[] SymptomId)
        {
            //if (!ModelState.IsValid)
            //{
            //    await LoadDropdowns(opd);
            //    return View(opd);
            //}

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.OPDs.Add(opd);
                await _context.SaveChangesAsync();

                if (DiagnosisId?.Any() == true)
                {
                    var opdDiagnoses = DiagnosisId
                        .Select(id => new OPDDiagnosis { OPDId = opd.Id, DiagnosisId = id })
                        .ToList();
                    _context.OPDDiagnoses.AddRange(opdDiagnoses);
                }

                if (SymptomId?.Any() == true)
                {
                    var opdSymptoms = SymptomId
                        .Select(id => new OPDSymptom { OPDId = opd.Id, SymptomId = id })
                        .ToList();
                    _context.OPDSymptoms.AddRange(opdSymptoms);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while saving the OPD.");
                await LoadDropdowns(opd);
                return View(opd);
            }
        }

        // ================== EDIT (GET) ==================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var opd = await _context.OPDs
                .Include(o => o.OPDDiagnoses)
                .Include(o => o.OPDSymptoms)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opd == null) return NotFound();

            await LoadDropdowns(opd);
            ViewBag.IsEdit = true;
            return View("Create", opd);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OPD opd, int[] DiagnosisId, int[] SymptomId)
        {
            //if (!ModelState.IsValid)
            //{
            //    await LoadDropdowns(opd);
            //    ViewBag.IsEdit = true;
            //    return View("Create", opd);
            //}

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingOpd = await _context.OPDs
                    .Include(o => o.OPDDiagnoses)
                    .Include(o => o.OPDSymptoms)
                    .FirstOrDefaultAsync(o => o.Id == opd.Id);

                if (existingOpd == null) return NotFound();

                existingOpd.PatientId = opd.PatientId;
                existingOpd.DoctorId = opd.DoctorId;
                existingOpd.Amount = opd.Amount;
                existingOpd.PaymentDate = opd.PaymentDate;

                _context.OPDDiagnoses.RemoveRange(existingOpd.OPDDiagnoses);
                if (DiagnosisId?.Any() == true)
                {
                    var newDiagnoses = DiagnosisId.Select(id => new OPDDiagnosis { OPDId = existingOpd.Id, DiagnosisId = id }).ToList();
                    _context.OPDDiagnoses.AddRange(newDiagnoses);
                }

                _context.OPDSymptoms.RemoveRange(existingOpd.OPDSymptoms);
                if (SymptomId?.Any() == true)
                {
                    var newSymptoms = SymptomId.Select(id => new OPDSymptom { OPDId = existingOpd.Id, SymptomId = id }).ToList();
                    _context.OPDSymptoms.AddRange(newSymptoms);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "An error occurred while updating the OPD.");
                await LoadDropdowns(opd);
                return View("Create", opd);
            }
        }

        // ================== DELETE ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var opd = await _context.OPDs.FindAsync(id);
            if (opd == null) return Json(new { success = false, message = "Record not found." });

            _context.OPDs.Remove(opd);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Record deleted successfully." });
        }

        // ================== GET ALL ==================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var opdList = await _context.OPDs
                .Include(o => o.Patient)
                .Include(o => o.Doctor)
                .Include(o => o.OPDDiagnoses).ThenInclude(d => d.Diagnosis)
                .Include(o => o.OPDSymptoms).ThenInclude(s => s.Symptom)
                .AsNoTracking()
                .Select(o => new
                {
                    id = o.Id,
                    patientName = o.Patient != null ? (o.Patient.FirstName + " " + o.Patient.LastName).Trim() : "",
                    doctorName = o.Doctor != null ? (o.Doctor.FirstName + " " + o.Doctor.LastName).Trim() : "",
                    diagnosisName = o.OPDDiagnoses.Any() ? string.Join(", ", o.OPDDiagnoses.Select(d => d.Diagnosis.Name)) : "",
                    symptomName = o.OPDSymptoms.Any() ? string.Join(", ", o.OPDSymptoms.Select(s => s.Symptom.Name)) : "",
                    amount = o.Amount,
                    paymentDate = o.PaymentDate.ToString("yyyy-MM-dd"),
                    invoiceNumber = o.InvoiceNumber
                })
                .ToListAsync();

            return Json(new { data = opdList });
        }

        // ================== Generate Invoice Number ==================
        private async Task<string> GenerateInvoiceNumber()
        {
            var lastOpd = await _context.OPDs.OrderByDescending(o => o.Id).FirstOrDefaultAsync();
            int nextNumber = 1;

            if (lastOpd != null && !string.IsNullOrEmpty(lastOpd.InvoiceNumber))
            {
                var numberPart = lastOpd.InvoiceNumber.Replace("NP", "");
                if (int.TryParse(numberPart, out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            return "NP" + nextNumber;
        }
    }
}
