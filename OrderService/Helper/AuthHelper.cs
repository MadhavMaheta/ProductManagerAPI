using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderService.Helper
{
    public static class AuthHelper
    {
        public static Dictionary<string, string> UserWiseTokens { get; set; } = new Dictionary<string, string>();

        public static string ValidateToken(IHeaderDictionary headers, string key)
        {
            string jwt = headers?["Authorization"].ToString().Replace("Bearer ", string.Empty);
            string val = string.Empty;
            var validationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateActor = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "false",
                ValidAudience = "MicroServiceAudience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TheMicroservicekeyy27t273")),
            };

            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler()
                    .ValidateToken(jwt, validationParameters, out var rawValidatedToken);
                var result = (JwtSecurityToken)rawValidatedToken;

                IdentityOptions options = new IdentityOptions();
                switch (key)
                {
                    case "Role":
                        val = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
                        break;

                    case "UserId":
                        val = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == options.ClaimsIdentity.UserIdClaimType).Value;
                        break;

                    case "RoleName":
                        val = claimsPrincipal.Claims.LastOrDefault(x => x.Type == options.ClaimsIdentity.RoleClaimType).Value;
                        break;

                    default:
                        break;
                }

                return val;
            }
            catch (SecurityTokenValidationException ex)
            {
                return string.Empty;
            }
            catch (ArgumentException ex)
            {
                return string.Empty;
            }
        }
    }
}
