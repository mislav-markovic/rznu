using lab1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lab1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly IUserService _userService;

		public LoginController(IUserService userService)
		{
			_userService = userService;
		}

		[AllowAnonymous]
		[HttpPost]
		public IActionResult Authenticate([FromBody]User userParam)
		{
			var user = _userService.Authenticate(userParam.Username, userParam.Password);

			if (user == null)
				return BadRequest(new { message = "Username or password is incorrect" });
			Response.Cookies.Append("Auth", "true");
			return Ok(user);
		}
	}
}