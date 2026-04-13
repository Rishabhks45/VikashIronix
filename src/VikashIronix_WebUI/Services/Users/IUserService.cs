using SharedKernel.DTOs.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VikashIronix_WebUI.Services.Users
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersAsync();
        Task CreateUserAsync(UserDto user);
        Task UpdateUserAsync(UserDto user);
        Task<List<RoleDto>> GetRolesAsync();
        Task CreateRoleAsync(RoleDto role);
        Task UpdateRoleAsync(RoleDto role);
        Task DeleteRoleAsync(int id);
        Task<UserDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(UserDto profile);
        Task ChangePasswordAsync(Guid userId, string newPassword);
    }
}
