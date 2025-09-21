using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class DoctorDeptController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorDeptController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DoctorDept
        public IActionResult Index()
        {
            return View();
        }

        // GET: DoctorDept/GetAll (for DataTables)
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Departments
                .Select(d => new { d.Id, d.DepartmentName, d.Description, d.Active })
                .ToListAsync();

            return Json(new { data });
        }

        // GET: DoctorDept/Create
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            return View("Create", new Department());
        }

        // POST: DoctorDept/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            
            if (department.Id == 0)
            {
                _context.Departments.Add(department);
            }
            else
            {
                _context.Update(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: DoctorDept/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            ViewBag.IsEdit = true;
            return View("Create", department);
        }

        // GET: DoctorDept/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null) return Json(new { success = false, message = "Department not found!" });

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Department deleted successfully!" });
        }
    }
}
