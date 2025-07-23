using HospitalManagement.Data;
using HospitalManagement.Helpers;
using HospitalManagement.Models;
using HospitalManagement.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password, IFormFile? imageFile, string[] hobbies)
        {
            user.Hobby = hobbies?.Length > 0 ? string.Join(", ", hobbies) : null;
            user.Image = await ImageHelper.SaveImageAsync(_webHostEnvironment, imageFile) ?? "user.png";

            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser model, IFormFile? imageFile, string[] hobbies)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return false;

            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.Hobby = hobbies?.Length > 0 ? string.Join(", ", hobbies) : null;
            user.RoleId = model.RoleId;

            if (user.Email != model.Email)
                await _userManager.SetEmailAsync(user, model.Email);
            if (user.UserName != model.UserName)
                await _userManager.SetUserNameAsync(user, model.UserName);

            if (imageFile != null && imageFile.Length > 0)
            {
                ImageHelper.DeleteImage(_webHostEnvironment, user.Image);
                user.Image = await ImageHelper.SaveImageAsync(_webHostEnvironment, imageFile);
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            ImageHelper.DeleteImage(_webHostEnvironment, user.Image);

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
