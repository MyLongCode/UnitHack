using Api.Controllers.User.Requests;
using Api.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UnitDal.Models;
using EFCore;
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
        /// <summary>
        /// Зарегать пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/user")]
        public IActionResult GetAllUsers()
        {
            User[] users = db.Users.ToArray();
            if (users == null)
                return BadRequest("users undefined");
            return Ok(users);
        }
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            User? user = db.Users.FirstOrDefault(x => x.Email == username && x.Password == password);
            if ((user == null) == false)
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

        /// <summary>
        /// Получить токен
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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
    }
}
