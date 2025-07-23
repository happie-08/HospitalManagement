using HospitalManagement.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<bool> CreateUserAsync(ApplicationUser user, string password, IFormFile? imageFile, string[] hobbies);
        Task<bool> UpdateUserAsync(ApplicationUser user, IFormFile? imageFile, string[] hobbies);
        Task<bool> DeleteUserAsync(string id);
    }
}
