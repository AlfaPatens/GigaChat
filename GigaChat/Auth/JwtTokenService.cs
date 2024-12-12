using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GigaChat.Auth
{
	public class JwtTokenService
	{
		private readonly SymmetricSecurityKey _authSignKey;
		private readonly string? _issuer;
		private readonly string? _audience;

		public JwtTokenService(IConfiguration configuration)
		{
			_authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]));
			_issuer = configuration["Jwt:ValidIssuer"];
			_audience = configuration["Jwt:ValidAudience"];

		}

		public string CreateAccessToken(string userName, string userID, IEnumerable<string> roles)
		{
			var authClaims = new List<Claim>
			{
				new(ClaimTypes.Name, userName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Sub, userID),
			};

			authClaims.AddRange(roles.Select(o => new Claim(ClaimTypes.Role, o)));

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				expires: DateTime.Now.AddMinutes(10),
				claims: authClaims,
				signingCredentials: new SigningCredentials(_authSignKey, SecurityAlgorithms.HmacSha256)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public string CreateRefreshToken(Guid sessionId, string userID, DateTime expires)
		{
			var authClaims = new List<Claim>
			{
				new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new (JwtRegisteredClaimNames.Sub, userID),
				new ("SessionId", sessionId.ToString())
			};


			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				expires: expires,
				claims: authClaims,
				signingCredentials: new SigningCredentials(_authSignKey, SecurityAlgorithms.HmacSha256)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public bool TryParseRefreshToken(string refreshToken, out ClaimsPrincipal? claims)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler() { MapInboundClaims = false };
				var validationParameters = new TokenValidationParameters()
				{
					ValidIssuer = _issuer,
					ValidAudience = _audience,
					IssuerSigningKey = _authSignKey,
					ValidateLifetime = true
				};

				claims = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
				return true;
			}
			catch (Exception e)
			{
				claims = null;
				return false;
			}
		}
	}
}
