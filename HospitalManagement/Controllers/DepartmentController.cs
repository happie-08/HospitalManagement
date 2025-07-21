using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [AllowAnonymous]
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            return View();
        }
        //Doctor
        public IActionResult Doctor()
        {
            return View();
        }

        public IActionResult DoctorSingle(int id)
        {
            return View();
        }

        public IActionResult Appointment()
        {
            return View();
        }
        //blog
        public IActionResult Sidebar()
        {
            return View();
        }

        public IActionResult Single(int id)
        {
            return View();
        }
    }
}
