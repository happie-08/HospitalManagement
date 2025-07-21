using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // ✅ Show User List Page
        public IActionResult Index()
        {
            return View();
        }

        // ✅ DataTables Ajax API
        [HttpGet]
        [Route("User/GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    phoneNumber = u.PhoneNumber,
                    userName = u.UserName,
                    gender = u.Gender,
                    hobby = u.Hobby,
                    address = u.Address ?? "-",
                    dateOfBirth = u.DateOfBirth.HasValue
                        ? u.DateOfBirth.Value.ToString("dd/MM/yyyy")
                        : "-",
                    image = string.IsNullOrEmpty(u.Image) ? "user.png" : u.Image
                })
                .ToListAsync();

            return Json(new { data = users });
        }

        // ✅ Show Create Form
        [HttpGet]
        [Route("User/Create")]
        public IActionResult Create()
        {
            return View(new ApplicationUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/Create")]
        public async Task<IActionResult> Create(ApplicationUser model, IFormFile? ImageFile, string[] Hobby, string Password)
        {
            if (ModelState.IsValid)
            {
                model.Hobby = Hobby != null && Hobby.Length > 0 ? string.Join(", ", Hobby) : null;

                // Save profile image only if uploaded
                string uniqueFileName = "user.png"; // default image
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                }

                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName ?? model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Gender = model.Gender,
                    Hobby = model.Hobby,
                    DateOfBirth = model.DateOfBirth,
                    Image = uniqueFileName
                };

                var result = await _userManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    TempData["success"] = "User created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please check all required fields.");
            }

            return View(model);
        }


        // ✅ Edit User - GET
        [HttpGet]
        [Route("User/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // ✅ Edit User - POST (Identity Safe)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/Edit/{id}")]
        public async Task<IActionResult> Edit(string id, ApplicationUser model, IFormFile? ImageFile, string[] Hobby)
        {
            if (id != model.Id)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Update fields
            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.Hobby = Hobby != null ? string.Join(", ", Hobby) : null;

            // Update Email and Username safely
            if (user.Email != model.Email)
                await _userManager.SetEmailAsync(user, model.Email);
            if (user.UserName != model.UserName)
                await _userManager.SetUserNameAsync(user, model.UserName);

            // ✅ Handle image upload (optional)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Delete old image if it's not default
                if (!string.IsNullOrEmpty(user.Image) && user.Image != "user.png")
                {
                    var oldImagePath = Path.Combine(uploadsFolder, user.Image);
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                // Save new image
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var newPath = Path.Combine(uploadsFolder, newFileName);
                using (var fileStream = new FileStream(newPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                user.Image = newFileName;
            }

            // ✅ If no new image uploaded, keep existing image
            if (string.IsNullOrEmpty(user.Image))
                user.Image = "user.png";

            // Save changes
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", "Update Error: " + error.Description);
                return View(model);
            }
        }

        // ✅ Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("User/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(new { success = false, message = "Invalid User ID" });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            // Delete image if not default
            if (!string.IsNullOrEmpty(user.Image) && user.Image != "user.png")
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string filePath = Path.Combine(uploadsFolder, user.Image);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Json(new { success = true, message = "User deleted successfully!" });

            return Json(new { success = false, message = "Delete failed!" });
        }
    }
}
