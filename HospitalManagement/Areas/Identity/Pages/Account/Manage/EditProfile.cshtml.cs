using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.Pages.Account.Manage
{
    public class EditProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public EditProfileModel(UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        [BindProperty]
        public ApplicationUser Input { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public SelectList RoleList { get; set; }
        private void LoadRoleList(string? selectedRoleId = null)
        {
            var roles = _roleManager.Roles.ToList();
            ViewData["RoleList"] = new SelectList(roles, "Id", "Name", selectedRoleId);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            Input = user;
            LoadRoleList(user.RoleId?.ToString());
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadRoleList(Input.RoleId?.ToString());
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // update fields
            user.Name = Input.Name;
            user.Email = Input.Email;
            user.UserName = Input.UserName;
            user.PhoneNumber = Input.PhoneNumber;
            user.DateOfBirth = Input.DateOfBirth;
            user.Gender = Input.Gender;
            user.RoleId = Input.RoleId;
            user.Hobby = Request.Form["Input.Hobby"];

            if (ImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                user.Image = fileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToPage("Index");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            LoadRoleList(Input.RoleId?.ToString());
            return Page();
        }

    }
}
