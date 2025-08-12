using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patient
        public async Task<IActionResult> Index()
        {
            var patients = _context.Patients.Include(p => p.ReferenceDoctor);
            return View(await patients.ToListAsync());
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _context.Patients
                .Include(p => p.ReferenceDoctor)
                .Select(p => new
                {
                    p.PatientId,
                    p.FirstName,
                    p.LastName,
                    p.Gender,
                    p.DateOfBirth,
                    p.Height,
                    p.Weight,
                    p.ContactNo,
                    p.Email,
                    p.Image,
                    ReferenceDoctor = p.ReferenceDoctor != null
                        ? p.ReferenceDoctor.FirstName + " " + p.ReferenceDoctor.LastName
                        : ""
                })
                .ToList();

            return Json(new { data });
        }

        // GET: Patient/Create
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            ViewBag.ReferenceDoctorId = new SelectList(_context.ReferenceDoctors.ToList(), "Id", "FirstName"); // or use FullName if you added it
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReferenceDoctorId = new SelectList(_context.ReferenceDoctors, "ReferenceDoctorId", "DoctorName", patient.ReferenceDoctorId);
            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            ViewBag.IsEdit = true;
            ViewBag.ReferenceDoctorId = new SelectList(_context.ReferenceDoctors.ToList(), "Id", "FirstName", patient.ReferenceDoctorId);
            return View("Create", patient); // reusing Create view
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Patients.Any(e => e.PatientId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReferenceDoctorId = new SelectList(_context.ReferenceDoctors, "ReferenceDoctorId", "DoctorName", patient.ReferenceDoctorId);
            return View(patient);
        }

        // GET: Patient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return Json(new { success = false, message = "Patient not found." });
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Patient deleted successfully." });
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
            .AsNoTracking()
            .Include(p => p.ReferenceDoctor)
            .FirstOrDefaultAsync(m => m.PatientId == id);


            if (patient == null) return NotFound();

            return View(patient);
        }
    }
}
