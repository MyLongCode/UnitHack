using Api.Controllers.User.Requests;
using Api.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UnitDal.Models;
using EFCore;
using UnitApi.dto.Order;
using UnitApi.dto.Item;

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

        [HttpGet]
        [Route("/user/{id}/order")]
        public IActionResult GetOrdersByUser(int id)
        {
            List<OrderDto> orders = new List<OrderDto>();
            foreach (var item in db.Orders.Where(o => o.UserId == id))
                orders.Add(GetOrderDtoById(item.Id));
            return Ok(orders);

        }
        private OrderDto GetOrderDtoById(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var order = db.Orders.Find(id);
                return new OrderDto
                {
                    Id = id,
                    UserId = order.UserId,
                    Items = order.ItemsId.Select(o => GetItemDtoById(o)).ToList(),
                    TotalCost = order.TotalCost,
                    Status = order.Status,
                    UserPhone = order.UserPhone,
                    UserEmail = order.UserEmail,
                    UserFullName = order.UserFullName,
                    DeliveryAddress = order.DeliveryAddress,
                    DeliveryDate = order.DeliveryDate,
                    CreatedTime = order.CreatedTime,
                };
            }
        }
        private ItemDto GetItemDtoById(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var item = db.Items.Find(id);
                return new ItemDto
                {
                    Title = item.Title,
                    Description = item.Description,
                    Cost = item.Cost,
                    Image = item.Image,
                    Category = item.Category,
                    Rating = item.Rating,

                };
            }
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
