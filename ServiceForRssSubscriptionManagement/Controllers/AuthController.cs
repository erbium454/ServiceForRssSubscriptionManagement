using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceForRssSubscriptionManagement.Models.AuthOptions;
using ServiceForRssSubscriptionManagement.Models.DataAccess.Auth;
using ServiceForRssSubscriptionManagement.Models.DataModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServiceForRssSubscriptionManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        AuthContext authDb;

        public AuthController() 
        {
            authDb = new AuthContext();
            authDb.Database.EnsureCreated();
        }

        JsonResult GetErrorResult() 
        {
            var fields = new Dictionary<string, List<string>>();

            foreach (var fld in ModelState)
            {
                foreach (var error in fld.Value.Errors)
                {
                    if (!fields.ContainsKey(fld.Key))
                        fields.Add(fld.Key, new List<string>());
                    fields[fld.Key].Add(error.ErrorMessage);
                }
            }
            return new JsonResult(fields);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var ph = new PasswordHasher<User>();

            authDb.Users.Add(new UserEntity
            {
                Email = user.Email,
                Password = ph.HashPassword(user, user.Password),
            });
            await authDb.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            var userEntity = await authDb.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == user.Email);
            if (userEntity == null) return NotFound("No user exists with this email");

            var ph = new PasswordHasher<User>();
            if (ph.VerifyHashedPassword(user, userEntity.Password, user.Password) != PasswordVerificationResult.Success)
                return Unauthorized();

            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Email) };

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromHours(AuthOptions.LIFETIMEHOURS)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JsonResult(new { token = new JwtSecurityTokenHandler().WriteToken(jwt) });
        }
    }
}
