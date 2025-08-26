using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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

        // GET: Patient/GetAll
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
                    p.Address,
                    p.Remark,
                    ReferenceDoctor = p.ReferenceDoctor != null
                        ? p.ReferenceDoctor.FirstName + " " + p.ReferenceDoctor.LastName
                        : ""
                }).ToList();

            return Json(new { data });
        }

        // GET: Patient/Create
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
         
            ViewBag.ReferenceDoctorId = new SelectList(_context.ReferenceDoctors
                .Select(d => new { d.Id, FullName = d.FirstName + " " + d.LastName })
                .ToList(), "Id", "FullName");
            var patient = new Patient { Gender = "Female" };
            return View(patient);
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    patient.Image = fileName;
                }

                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            ViewBag.IsEdit = true;
            ViewBag.ReferenceDoctorId = new SelectList(_context.ReferenceDoctors
                .Select(d => new { d.Id, FullName = d.FirstName + " " + d.LastName })
                .ToList(), "Id", "FullName", patient.ReferenceDoctorId);

            return View("Create", patient); // Reuse Create view
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient, IFormFile? ImageFile)
        {
            var existing = await _context.Patients.FindAsync(id);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                // update properties except Image
                _context.Entry(existing).CurrentValues.SetValues(patient);

                // handle image replacement
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // delete old
                    if (!string.IsNullOrEmpty(existing.Image))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", existing.Image);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    existing.Image = fileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(patient);
        }


        // POST: Patient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return Json(new { success = false, message = "Patient not found!" });
            }

            // delete image
            if (!string.IsNullOrEmpty(patient.Image))
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", patient.Image);
                if (System.IO.File.Exists(imgPath)) System.IO.File.Delete(imgPath);
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Patient deleted successfully!" });
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
