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
        private void LoadDropdowns()
        {
            ViewBag.Patients = new SelectList(
                _context.Patients
                    .Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName })
                    .ToList(),
                "PatientId",
                "FullName"
            );

            ViewBag.Doctors = new SelectList(
                _context.ReferenceDoctors
                    .Select(d => new { d.Id, FullName = d.FirstName + " " + d.LastName })
                    .ToList(),
                "Id",
                "FullName"
            );

            ViewBag.DiagnosisList = new SelectList(
                _context.Masters.Where(m => m.Type == "Diagnosis").ToList(),
                "Id",
                "Name"
            );

            ViewBag.SymptomList = new SelectList(
                _context.Masters.Where(m => m.Type == "Symptoms").ToList(),
                "Id",
                "Name"
            );
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
                    paymentDate = o.PaymentDate.HasValue
                    ? o.PaymentDate.Value.ToString("yyyy-MM-dd")
                    : "",
                    investNumber = o.InvestNumber
                })
                .ToList();

            return Json(new { data = opdList });
        }

        // ================== CREATE (GET) ==================
        [HttpGet]
        public IActionResult Create()
        {
            var opd = new OPD
            {
                PaymentDate = DateTime.Today // ✅ default date
            };
            LoadDropdowns();
            ViewBag.IsEdit = false;
            return View(opd);
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OPD opd)
        {
            if (ModelState.IsValid)
            {
                _context.OPDs.Add(opd);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            LoadDropdowns();
            ViewBag.IsEdit = false;
            return View(opd);
        }

        // ================== EDIT (GET) ==================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var opd = _context.OPDs.Find(id);
            if (opd == null)
                return NotFound();

            LoadDropdowns();
            ViewBag.IsEdit = true;
            return View("Create", opd); // ✅ reuse Create view
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OPD opd)
        {
            if (ModelState.IsValid)
            {
                _context.OPDs.Update(opd);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            LoadDropdowns();
            ViewBag.IsEdit = true;
            return View("Create", opd);
        }

        // ================== DELETE ==================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var opd = _context.OPDs.Find(id);
            if (opd == null)
                return Json(new { success = false, message = "Record not found." });

            _context.OPDs.Remove(opd);
            _context.SaveChanges();
            return Json(new { success = true, message = "Record deleted successfully." });
        }
    }
}
