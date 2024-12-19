using GigaChat.Auth.Model;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GigaChat.Auth
{
	public static class AuthEndpoints
	{
		public static void AddAuthApi(this WebApplication app)
		{
			//Register
			app.MapPost("api/v1/accounts", async (UserManager<ForumUser> userManager, RegisterUserDto dto) =>
			{
				var user = await userManager.FindByNameAsync(dto.UserName);
				if (user != null)
				{
					return Results.UnprocessableEntity("Username already taken");
				}

				var newUser = new ForumUser()
				{
					Email = dto.Email,
					UserName = dto.UserName
				};
				// TOOD: wrap in transaction
				var createdUserResult = await userManager.CreateAsync(newUser, dto.Password);

				if (!createdUserResult.Succeeded)
				{
					return Results.UnprocessableEntity();
				}

				await userManager.AddToRoleAsync(newUser, ForumRoles.ForumUser);

				return Results.Created();
			});

			//Login
			app.MapPost("api/v1/login", async (UserManager<ForumUser> userManager, JwtTokenService jwtTokenService, SessionService sessionService, HttpContext httpContext, LoginUserDto dto) =>
			{
				var user = await userManager.FindByNameAsync(dto.UserName);
				if (user == null)
				{
					return Results.UnprocessableEntity("Username does not exist");
				}

				var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);

				if (!isPasswordValid)
				{
					return Results.UnprocessableEntity("Username or password was Incorrect");
				}

				var roles = await userManager.GetRolesAsync(user);

				var sessionId = Guid.NewGuid();
				var expiresAt = DateTime.UtcNow.AddDays(3);
				var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
				var refreshToken = jwtTokenService.CreateRefreshToken(sessionId, user.Id, expiresAt);

				await sessionService.CreateSessionAsync(sessionId, user.Id, refreshToken, expiresAt);

				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					SameSite = SameSiteMode.Lax,
					Expires = expiresAt,
					//Secure = false
				};

				httpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

				return Results.Ok(new SuccessfulLoginDto(accessToken));
			});

			//Refresh tokens (session)
			app.MapPost("api/v1/accessToken", async (UserManager<ForumUser> userManager, JwtTokenService jwtTokenService, SessionService sessionService, HttpContext httpContext) =>
			{
				if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
				{
					return Results.UnprocessableEntity();
				}

				if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
				{
					return Results.UnprocessableEntity();
				}

				var sessionId = claims.FindFirstValue("SessionId");
				if (string.IsNullOrWhiteSpace(sessionId))
				{
					return Results.UnprocessableEntity();
				}

				var sessionIdAsGuid = Guid.Parse(sessionId);
				if (!await sessionService.IsSessionValidAsync(sessionIdAsGuid, refreshToken))
				{
					return Results.UnprocessableEntity();
				}

				var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
				var user = await userManager.FindByIdAsync(userId);

				if (user == null)
				{
					return Results.UnprocessableEntity();
				}

				var roles = await userManager.GetRolesAsync(user);

				var expiresAt = DateTime.UtcNow.AddDays(3);
				var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
				var newRefreshToken = jwtTokenService.CreateRefreshToken(sessionIdAsGuid, user.Id, expiresAt);

				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					SameSite = SameSiteMode.Lax,
					Expires = expiresAt,
					//Secure = false
				};

				httpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, cookieOptions);

				await sessionService.ExtendSessionAsync(sessionIdAsGuid, newRefreshToken, expiresAt);

				return Results.Ok(new SuccessfulLoginDto(accessToken));
			});

			//Logout
			app.MapPost("api/v1/logout", async (UserManager<ForumUser> userManager, JwtTokenService jwtTokenService, SessionService sessionService, HttpContext httpContext) =>
			{
				if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
				{
					return Results.UnprocessableEntity();
				}

				if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
				{
					return Results.UnprocessableEntity();
				}

				var sessionId = claims.FindFirstValue("SessionId");
				if (string.IsNullOrWhiteSpace(sessionId))
				{
					return Results.UnprocessableEntity();
				}

				var sessionIdAsGuid = Guid.Parse(sessionId);

				await sessionService.InvalidateSessionAsync(sessionIdAsGuid);
				httpContext.Response.Cookies.Delete("RefreshToken");

				return Results.Ok();
			});
			app.MapGet("api/v1/accounts/{userId}", async (UserManager<ForumUser> userManager, string userId) =>
			{
				var user = await userManager.FindByIdAsync(userId);
				if (user == null)
				{
					return Results.NotFound("User not found");
				}
				return Results.Ok(new { UserName = user.UserName });
			});
		}

		public record RegisterUserDto(string UserName, string Email, string Password);
		public record LoginUserDto(string UserName, string Password);
		public record SuccessfulLoginDto(string AccessToken);
	}
}
