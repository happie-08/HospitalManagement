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
                .Select(r => new
                {
                    r.Id,
                    r.HospitalName,
                    r.FirstName,
                    r.LastName,
                    r.Address,
                    r.Degree,
                    r.ContactNo,
                    r.Email,
                    r.Active
                }).ToListAsync();

            return Json(new { data });
        }

        // GET: /ReferenceDoctor/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            return View();
        }

        // POST: /ReferenceDoctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReferenceDoctor model)
        {
            if (ModelState.IsValid)
            {
                _context.ReferenceDoctors.Add(model);
                await _context.SaveChangesAsync();
                TempData["success"] = "Reference Doctor added successfully!";
                return RedirectToAction("Index");
            }

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
            return View("Create", doctor); // Reuse the same view
        }

        // POST: /ReferenceDoctor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReferenceDoctor model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReferenceDoctors.Any(e => e.Id == model.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.IsEdit = true;
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
