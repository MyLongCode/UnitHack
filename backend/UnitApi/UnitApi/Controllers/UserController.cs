using Api.Controllers.User.Requests;
using Api.Models;
using Dal.EF;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UnitApi.Models;

namespace UnitApi.Controllers
{
    [Route("/user")]
    public class UserController : Controller
    {
        ApplicationDbContext db;
        public UserController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpPost]
        [Route("/user/register")]
        public IActionResult RegisterUser(UserRegisterRequest dto)
        {
            var userFound = db.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (userFound != null) return BadRequest("user with this email is already register");
            var user = new User
            {
                Email = dto.Email,
                Password = dto.Password,
                FullName = dto.FullName,
                Birthday = dto.Birthday,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role
            };
            db.Users.Add(user);
            db.SaveChanges();
            
            return Ok("user created");
        }
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            if (CheckUserPassword(username, password) != false)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                    //new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [HttpPost("/user/token")]
        public IActionResult Token(GetUserTokenRequest dto)
        {
            var identity = GetIdentity(dto.Email, dto.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid email or password." });
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

        public bool CheckUserPassword(string email, string password)
        {
            User? user = db.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            return !(user == null);
        }
    }
}
