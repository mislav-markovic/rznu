using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using lab1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace lab1
{
	public interface IUserService
	{
		User Authenticate(string username, string password);
	}

	public class UserService : IUserService
	{
		private readonly List<User> _users = new List<User>
		{
			new User {Username = "test", Password = "test"}
		};

		public User Authenticate(string username, string password)
		{
			var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

			// return null if user not found
			if (user == null)
				return null;

			// authentication successful so return user details without password
			user.Password = null;
			return user;
		}
	}

	public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly IUserService _userService;

		public BasicAuthenticationHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock,
			IUserService userService)
			: base(options, logger, encoder, clock)
		{
			_userService = userService;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, "user")
			};
			var identity = new ClaimsIdentity(claims, Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);

			var cookie = Request.Cookies["Auth"] ?? "false";
			if (cookie.Equals("true")) return AuthenticateResult.Success(ticket);
			return AuthenticateResult.Fail("Not authorized");
		}
	}
}