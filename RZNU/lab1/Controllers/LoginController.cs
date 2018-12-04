using lab1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

		[SwaggerOperation(Summary = "Login", Description = "Send username and password to login and gain access to the rest of methods.", OperationId = "Login")]
		[SwaggerResponse(400, "Incorrect username/password combination")]
		[SwaggerResponse(200, "Successfully logged in.")]
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