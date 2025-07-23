using HospitalManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<bool> RoleNameExistsAsync(string name, int? excludeId = null);
        Task AddRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int id);
    }

}
