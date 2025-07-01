using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APIWithRBACJWT.Configuarations
{
	public static class AuthConfigurator
	{
		public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
		{
			services.Configure<JwtSettings>(config.GetSection("Jwt"));
			var jwt = config.GetSection("Jwt").Get<JwtSettings>();

			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(opt =>
			{
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwt.Issuer,
					ValidAudience = jwt.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
				};
			});

			services.AddAuthorization();
			return services;
		}

	}
}
