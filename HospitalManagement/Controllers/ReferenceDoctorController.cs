using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class ReferenceDoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReferenceDoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ReferenceDoctor
        public IActionResult Index()
        {
            return View();
        }

        // GET: /ReferenceDoctor/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.ReferenceDoctors
                    .Include(r => r.Department)
                .Select(r => new
                {
                    r.Id,
                    r.Image,
                    r.HospitalName,
                    r.FirstName,
                    r.LastName,
                    r.Address,
                    r.Degree,
                    r.ContactNo,
                    r.Email,
                    r.Active,
                    Department = new { r.Department.Id, r.Department.DepartmentName },
                }).ToListAsync();

            return Json(new { data });
        }

        // GET: /ReferenceDoctor/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        // POST: /ReferenceDoctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReferenceDoctor model, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string folderPath = Path.Combine("wwwroot", "uploads", "doctor");
                    Directory.CreateDirectory(folderPath); // Ensure folder exists
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    model.Image = "/uploads/doctor/" + fileName; // Save relative path
                }

                _context.ReferenceDoctors.Add(model);
                await _context.SaveChangesAsync();
                TempData["success"] = "Reference Doctor added successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.Departments = _context.Departments.ToList(); // 👈 reload on failure
            return View(model);
        }

        // GET: /ReferenceDoctor/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _context.ReferenceDoctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            ViewBag.IsEdit = true;
            ViewBag.Departments = _context.Departments.ToList();
            return View("Create", doctor); // Reuse the same view
        }

        // POST: /ReferenceDoctor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReferenceDoctor model, IFormFile? ImageFile)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var doctor = await _context.ReferenceDoctors.FindAsync(id);
                if (doctor == null)
                    return NotFound();

                // Update fields
                doctor.FirstName = model.FirstName;
                doctor.LastName = model.LastName;
                doctor.HospitalName = model.HospitalName;
                doctor.Degree = model.Degree;
                doctor.ContactNo = model.ContactNo;
                doctor.Email = model.Email;
                doctor.Address = model.Address;
                doctor.Active = model.Active;

                // Handle new image
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // save new file
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string folderPath = Path.Combine("wwwroot", "uploads", "doctor");
                    Directory.CreateDirectory(folderPath);
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // delete old image
                    if (!string.IsNullOrEmpty(doctor.Image))
                    {
                        string oldImagePath = Path.Combine("wwwroot", doctor.Image.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    doctor.Image = "/uploads/doctor/" + fileName; // new image
                }
                else
                {
                    // ✅ keep existing image if no new file uploaded
                    doctor.Image = model.Image;
                }

                await _context.SaveChangesAsync();
                TempData["success"] = "Reference Doctor updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsEdit = true;
            ViewBag.Departments = _context.Departments.ToList(); // 👈 reload on failure
            return View("Create", model);
        }


        // DELETE: /ReferenceDoctor/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await _context.ReferenceDoctors.FindAsync(id);
            if (doc == null)
                return Json(new { success = false, message = "Reference Doctor not found" });

            _context.ReferenceDoctors.Remove(doc);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
    }
}
