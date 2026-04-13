using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.DTOs.Users;

namespace WebApi.Features.Users;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UsersService _service;
    
    public UsersController(UsersService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken token)
    {
        var users = await _service.GetUsersAsync(token);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserDto user, CancellationToken token)
    {
        await _service.CreateUserAsync(user, token);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto user, CancellationToken token)
    {
        await _service.UpdateUserAsync(user, token);
        return Ok();
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken token)
    {
        var roles = await _service.GetRolesAsync(token);
        return Ok(roles);
    }

    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto role, CancellationToken token)
    {
        await _service.CreateRoleAsync(role, token);
        return Ok();
    }

    [HttpPut("roles")]
    public async Task<IActionResult> UpdateRole([FromBody] RoleDto role, CancellationToken token)
    {
        await _service.UpdateRoleAsync(role, token);
        return Ok();
    }

    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken token)
    {
        await _service.DeleteRoleAsync(id, token);
        return Ok();
    }

    [HttpGet("profile/{userId}")]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var profile = await _service.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserDto profile)
    {
        await _service.UpdateProfileAsync(profile);
        return Ok();
    }

    [HttpPost("profile/change-password")]
    public async Task<IActionResult> ChangePassword([FromQuery] Guid userId, [FromQuery] string newPassword)
    {
        await _service.ChangePasswordAsync(userId, newPassword);
        return Ok();
    }
}
