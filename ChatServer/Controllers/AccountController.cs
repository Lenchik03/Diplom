using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectSystemAPI.DB;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ProjectSystemNewContext _context;

        public AccountController(ProjectSystemNewContext context)
        {
            _context = context;
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Ok(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            ChatServerDTO.DB.User person = await _context.Users.Include(s => s.IdRoleNavigation).FirstOrDefaultAsync(x => x.Email == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.IdRoleNavigation.Title)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователь не найден
            return null;
        }
    }
}
