using GClaims.Core.Auth;
using GClaims.Domain.Models.Auth.Users;
using Microsoft.AspNetCore.Mvc;

namespace GClaims.Host.Controllers.Core
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        public LoginService LoginService { get; set; }

        public AuthController(
            LoginService loginService)
        {
            LoginService = loginService;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginInput input)
        {
            return Ok(LoginService.Login(new AppUser { Email = input.Email, Password = input.Password}));
        }
    }

    public class LoginInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
            
    }
}
