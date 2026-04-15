using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Extensions;
using ZeroTrustAPI.Api.Mappers;
using ZeroTrustAPI.Api.Repositories.Interfaces;

namespace ZeroTrustAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    public UsersController(IUserRepository userRepository) => _userRepository = userRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users.Select(UserMapper.ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(UserMapper.ToDto(user));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return NotFound();
        return Ok(UserMapper.ToDto(user));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        if (!string.IsNullOrEmpty(request.Username)) user.Username = request.Username;
        if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
        await _userRepository.UpdateAsync(user);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userRepository.DeleteAsync(id);
        return NoContent();
    }
}