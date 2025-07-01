using APIWithRBACJWT.Configuarations;
using APIWithRBACJWT.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIWithRBACJWT.Services
{
	public class JwtTokenService
	{
		private readonly JwtSettings _jwt;

		public JwtTokenService(IOptions<JwtSettings> jwtOptions)
		{
			_jwt = jwtOptions.Value;
		}

		public string GenerateToken(ApplicationUser user, IList<string> roles)
		{
			var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id),
			new(ClaimTypes.Email, user.Email!),
			new(ClaimTypes.Name, user.FullName!)
		};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwt.Issuer,
				audience: _jwt.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
}
