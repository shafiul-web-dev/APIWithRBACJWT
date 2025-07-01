using APIWithRBACJWT.DTO;
using APIWithRBACJWT.Models;
using APIWithRBACJWT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIWithRBACJWT.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly JwtTokenService _jwt;

		public AuthController(UserManager<ApplicationUser> userManager, JwtTokenService jwt)
		{
			_userManager = userManager;
			_jwt = jwt;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDto dto)
		{
			var user = new ApplicationUser
			{
				FullName = dto.FullName,
				Email = dto.Email,
				UserName = dto.Email,
				PhoneNumber = dto.PhoneNumber
			};

			var result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
				return BadRequest(result.Errors);

			await _userManager.AddToRoleAsync(user, "User");
			return Ok("User registered successfully");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
				return Unauthorized("Invalid credentials");

			var roles = await _userManager.GetRolesAsync(user);
			var token = _jwt.GenerateToken(user, roles);

			return Ok(new { token });
		}

	}
}
