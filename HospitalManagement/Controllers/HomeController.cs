using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace HospitalManagement.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var roleName = await _context.Users
                    .Where(u => u.Id == userId)
                    .Join(_context.Roles,
                          u => u.RoleId,
                          r => r.Id,
                          (u, r) => r.Name)
                    .FirstOrDefaultAsync();
            }

            ViewData["Title"] = "Home";
            return View(); // render for all users
        }


        public IActionResult About()
        {
            ViewData["Title"] = "About";
            return View();
        }

        public IActionResult Service()
        {
            ViewData["Title"] = "Services";
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
