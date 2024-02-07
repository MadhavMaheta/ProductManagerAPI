using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IConfiguration _config;
        public IUserService _userService { get; set; }

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel user)
        {
            User userAvailable = _userService.GetUser(user.Username, user.Password);

            if (userAvailable == null || userAvailable.Id == 0) { return Unauthorized(); }
            
            user.id = userAvailable.Id;
            user.Username = userAvailable.Name;
            user.Password = userAvailable.Password;
            user.role = userAvailable.Role == Role.Admin ? "Admin" : "Customer";

            var loginResult = PreparejwtToken(user, user.role);

            return loginResult is null ? Unauthorized() : Ok(loginResult);
        }

        [NonAction]
        private LoginRespone PreparejwtToken(LoginModel model, string userRole, string refreshToken = null)
        {
            string[] permissions = null;
            var response = new LoginRespone();
            var jti = Guid.NewGuid().ToString();

            IdentityOptions _options = new IdentityOptions();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.GivenName, model.Username),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, model.Username.ToString()),
                new Claim(ClaimTypes.Role,userRole),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("TokenAuthentication:SecretKey").Value));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(
                    issuer: _config.GetSection("TokenAuthentication:Issuer").Value,
                    audience: _config.GetSection("TokenAuthentication:Audience").Value,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signingCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            response = new LoginRespone()
            {
                UserFirstName = model.Username,
                JwtToken = token,
                Expiration = securityToken.ValidTo,
                StatusCode = (int)HttpStatusCode.OK,
                UserId = model.id,
                //RoleId = model.roleId,
                RoleName = userRole,
            };
            return response;
        }
    }

    public class LoginModel
    {
        public long id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string role { get; set; }
        //public int roleId { get; set; }
    };

}
