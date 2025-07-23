using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HospitalManagement.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet(string returnUrl = null) // 👈 Add OnGet
        {
            await LogoutUserAsync();
            return RedirectToAction("Index", "Home"); // 👈 Redirect to MVC Home/Index
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await LogoutUserAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home"); // 👈 Redirect to MVC Home/Index
        }

        private async Task LogoutUserAsync()
        {
            HttpContext.Session.Clear(); // ✅ Clear session
            Response.Cookies.Delete(".AspNetCore.Identity.Application"); // ✅ Clear identity cookie
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }
    }
}
