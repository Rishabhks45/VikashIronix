using SharedKernel.DTOs.Users;
using WebApi.Features.Users.Infrastructure;

namespace WebApi.Features.Users;

public class UsersService
{
    private readonly UsersRepository _repository;

    public UsersService(UsersRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UserDto>> GetUsersAsync(CancellationToken token)
    {
        return await _repository.GetUsersAsync(token);
    }

    public async Task CreateUserAsync(UserDto user, CancellationToken token)
    {
        await _repository.CreateUserAsync(user, token);
    }

    public async Task UpdateUserAsync(UserDto user, CancellationToken token)
    {
        await _repository.UpdateUserAsync(user, token);
    }

    public async Task<List<RoleDto>> GetRolesAsync(CancellationToken token)
    {
        return await _repository.GetRolesAsync(token);
    }

    public async Task CreateRoleAsync(RoleDto role, CancellationToken token)
    {
        await _repository.CreateRoleAsync(role, token);
    }

    public async Task UpdateRoleAsync(RoleDto role, CancellationToken token)
    {
        await _repository.UpdateRoleAsync(role, token);
    }

    public async Task DeleteRoleAsync(int id, CancellationToken token)
    {
        await _repository.DeleteRoleAsync(id, token);
    }

    public async Task<UserDto?> GetProfileAsync(Guid userId)
    {
        return await _repository.ManageProfileAsync("GET", userId: userId);
    }

    public async Task UpdateProfileAsync(UserDto profile)
    {
        await _repository.ManageProfileAsync("UPDATE", userId: profile.Id, profile: profile);
    }

    public async Task ChangePasswordAsync(Guid userId, string newPassword)
    {
        await _repository.ManageProfileAsync("CHANGE_PASSWORD", userId: userId, newPassword: newPassword);
    }
}
