using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using APIWithRBACJWT.Models;
using APIWithRBACJWT.DTO;
using APIWithRBACJWT.DTO.APIWithRBACJWT.DTO;

namespace APIWithRBACJWT.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;

	public UserController(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> GetMyProfile()
	{
		var email = User.FindFirstValue(ClaimTypes.Email);
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null) return NotFound("User not found");

		var roles = await _userManager.GetRolesAsync(user);

		var profile = new UserProfileDto
		{
			Id = user.Id,
			FullName = user.FullName,
			Email = user.Email!,
			PhoneNumber = user.PhoneNumber!,
			Roles = roles.ToList()
		};

		return Ok(profile);
	}


	[HttpPut("me")]
	[Authorize]
	public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
	{
		var email = User.FindFirstValue(ClaimTypes.Email);
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null) return NotFound();

		if (!string.IsNullOrWhiteSpace(dto.FullName))
			user.FullName = dto.FullName;

		if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
			user.PhoneNumber = dto.PhoneNumber;

		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded) return BadRequest(result.Errors);

		return Ok("Your profile has been updated.");
	}


	[Authorize(Roles = "Admin")]
	[HttpGet]
	public IActionResult GetAllUsers()
	{
		var users = _userManager.Users.Select(u => new UserProfileDto
		{
			Id = u.Id,
			FullName = u.FullName,
			Email = u.Email!,
			PhoneNumber = u.PhoneNumber!,
			Roles = new List<string>() 
		}).ToList();

		return Ok(users);
	}


	[Authorize(Roles = "Admin")]
	[HttpGet("{id}")]
	public async Task<IActionResult> GetUserById(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user is null) return NotFound();

		var roles = await _userManager.GetRolesAsync(user);

		var profile = new UserProfileDto
		{
			Id = user.Id,
			FullName = user.FullName,
			Email = user.Email!,
			PhoneNumber = user.PhoneNumber!,
			Roles = roles.ToList()
		};

		return Ok(profile);
	}


	[Authorize(Roles = "Admin")]
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateUserById(string id, [FromBody] RegisterDto dto)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user is null) return NotFound();

		user.FullName = dto.FullName;
		user.Email = dto.Email;
		user.UserName = dto.Email;
		user.PhoneNumber = dto.PhoneNumber;

		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded) return BadRequest(result.Errors);

		return Ok("User updated successfully.");
	}

	[Authorize(Roles = "Admin")]
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteUser(string id)
	{
		var user = await _userManager.FindByIdAsync(id);
		if (user is null) return NotFound();

		var result = await _userManager.DeleteAsync(user);
		if (!result.Succeeded) return BadRequest(result.Errors);

		return Ok("User deleted.");
	}
}