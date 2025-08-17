    using HospitalManagement.Data;
    using HospitalManagement.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    namespace HospitalManagement.Controllers
    {
        public class MasterController : Controller
        {
            private readonly ApplicationDbContext _context;

            public MasterController(ApplicationDbContext context)
            {
                _context = context;
            }
            private List<SelectListItem> GetTypeList()
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Value = "Symptoms", Text = "Symptoms" },
                    new SelectListItem { Value = "Diagnosis", Text = "Diagnosis" }
                };
            }
            public IActionResult Index()
            {
                return View();
            }

            [HttpGet]
            public IActionResult GetMasters()
            {
                var data = _context.Masters.ToList();
                return Json(new { data });
            }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.TypeList = GetTypeList();
            var model = new Master
            {
                Active = true // default checked
            };
            ViewBag.IsEdit = false;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Master master)
        {
            if (ModelState.IsValid)
            {
                _context.Masters.Add(master);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TypeList = GetTypeList();
            ViewBag.IsEdit = false;
            return View(master);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var master = _context.Masters.Find(id);
            if (master == null)
                return NotFound();

            ViewBag.TypeList = GetTypeList();
            ViewBag.IsEdit = true;
            return View("Create", master);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Master master)
        {
            if (ModelState.IsValid)
            {
                _context.Update(master);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TypeList = GetTypeList();
            ViewBag.IsEdit = true;
            return View("Create", master);
        }

        [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Delete(int id)
            {
                var master = _context.Masters.Find(id);
                if (master == null)
                    return Json(new { success = false, message = "Master not found!" });

                _context.Masters.Remove(master);
                _context.SaveChanges();
                return Json(new { success = true, message = "Master deleted successfully!" });
            }
        }
    }
