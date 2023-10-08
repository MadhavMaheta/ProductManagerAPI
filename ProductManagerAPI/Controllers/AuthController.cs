using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductManagerAPI.EFCore;
using ProductManagerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _configuration;
        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public ActionResult<Token> Login(User user)
        {
            var objUser = _context.User.Where(x=>x.Name == user.Name && x.Password == user.Password).FirstOrDefault();
            if (objUser == null)
            {
                return NotFound();
            }

            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", objUser.Id.ToString()),
                        new Claim("DisplayName", objUser.Name),
                        new Claim("UserName", objUser.Name),
                        new Claim("Email", objUser.Email)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            Token objToken = new Token();
            objToken.AuthToken = new JwtSecurityTokenHandler().WriteToken(token);
            return objToken;
        }
    }
}
