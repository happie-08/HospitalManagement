using HospitalManagement.Models;
using HospitalManagement.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;

        public UserController(IUserRepository userRepo, IRoleRepository roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        // ✅ Show User List Page
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Get Users for DataTable
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepo.GetAllUsersAsync();
            var jsonData = users.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                phoneNumber = u.PhoneNumber,
                userName = u.UserName,
                roleName = u.Role?.Name ?? "No Role",
                gender = u.Gender,
                hobby = u.Hobby,
                address = u.Address ?? "-",
                dateOfBirth = u.DateOfBirth?.ToString("dd/MM/yyyy") ?? "-",
                image = string.IsNullOrEmpty(u.Image) ? "user.png" : u.Image
            });

            return Json(new { data = jsonData });
        }

        // ✅ Show Create Form
        public async Task<IActionResult> Create()
        {
            await LoadRolesDropdown();
            ViewBag.IsEdit = false;
            return View("Create", new ApplicationUser());
        }

        // ✅ Handle Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser model, IFormFile? ImageFile, string[] Hobby, string Password)
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesDropdown(model.RoleId);
                ViewBag.IsEdit = false;
                return View("Create", model);
            }

            var isCreated = await _userRepo.CreateUserAsync(model, Password, ImageFile, Hobby);
            if (isCreated)
            {
                TempData["success"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to create user.");
            await LoadRolesDropdown(model.RoleId);
            ViewBag.IsEdit = false;
            return View("Create", model);
        }

        // ✅ Show Edit Form
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await LoadRolesDropdown(user.RoleId);
            ViewBag.IsEdit = true;
            return View("Create", user);
        }

        // ✅ Handle Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model, IFormFile? ImageFile, string[] Hobby)
        {
            if (id != model.Id) return NotFound();

            var isUpdated = await _userRepo.UpdateUserAsync(model, ImageFile, Hobby);
            if (isUpdated)
            {
                TempData["success"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to update user.");
            await LoadRolesDropdown(model.RoleId);
            ViewBag.IsEdit = true;
            return View("Create", model);
        }

        // ✅ Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var isDeleted = await _userRepo.DeleteUserAsync(id);
            if (isDeleted)
                return Json(new { success = true, message = "User deleted successfully!" });

            return Json(new { success = false, message = "Failed to delete user!" });
        }

        private async Task LoadRolesDropdown(int? selectedRoleId = null)
        {
            var roles = await _roleRepo.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", selectedRoleId);
        }
    }
}
