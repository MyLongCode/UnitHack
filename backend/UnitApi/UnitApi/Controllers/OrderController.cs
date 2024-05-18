using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using UnitApi.dto.Order;
using Microsoft.AspNetCore.Components.Web;
using UnitDal.Models;
using EFCore;
using Org.BouncyCastle.Crypto.Operators;

namespace UnitApi.Controllers
{
    public class OrderController : Controller
    {
        ApplicationDbContext db;
        public OrderController(ApplicationDbContext context)
        {
            db = context;
        }
        /// <summary>
        /// Создать заказ
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/order")]
        public IActionResult CreateOrder(CreateOrderRequest dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                ItemsId = dto.ItemsId,
                Status = "Создан",
                UserPhone = dto.UserPhone,
                UserEmail = dto.UserEmail,
                UserFullName = dto.UserFullName,
                DeliveryAddress = dto.DeliveryAddress,
                DeliveryDate = dto.DeliveryDate,
                CreatedTime = DateTime.Now,
                TotalCost = dto.ItemsId.Select(i => db.Items.Find(i).Cost).Sum()
            };

            db.Orders.Add(order);
            db.SaveChanges();
            string[] titleItems = dto.ItemsId.Select(i => db.Items.Find(i).Title).ToArray();
            SendEmailAsync(dto.UserEmail, String.Join(", ", titleItems), dto.DeliveryAddress, dto.DeliveryDate);
            return Ok("order is created");
        }

        [HttpGet]
        [Route("/order/{id}")]
        [ProducesResponseType(typeof(GetOrderResponse), 200)]
        public IActionResult GetAllOrders(int id)
        {
            var order = db.Orders.Find(id);
            return Ok(new GetOrderResponse
            {
                Items = order.ItemsId.Select(o => GetItemDtoById(o)).ToList(),
                TotalCost = order.TotalCost,
                Status = order.Status,
                UserPhone = order.UserPhone,
                UserEmail = order.UserEmail,
                UserFullName = order.UserFullName,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryDate = order.DeliveryDate,
                CreatedTime = order.CreatedTime,
            });
        }

        private ItemDto GetItemDtoById(int id)
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

        public async Task SendEmailAsync(string email, string titleItems, string deliveryAddress, DateTime delivaryDate)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация интернет магазина Warpoint", "mylongcode2@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("Работает!", email));
            emailMessage.Subject = "Ваш заказ подтверждён!";
            string htmlContent = $"\r\n<body style=\"background-color:white\"> \r\n    <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"\r\n           width=\"550\" bgcolor=\"white\" style=\"border:2px solid black\"> \r\n        <tbody> \r\n            <tr> \r\n                <td align=\"center\"> \r\n                    <table align=\"center\" border=\"0\" cellpadding=\"0\" \r\n                           cellspacing=\"0\" class=\"col-550\" width=\"550\"> \r\n                        <tbody> \r\n                            <tr> \r\n                                <td align=\"center\" style=\"background-color: #E23239; \r\n                                           height: 50px;\"> \r\n  \r\n                                    <a href=\"#\" style=\"text-decoration: none;\"> \r\n                                        <p style=\"color:#121212; \r\n                                                  font-weight:bold;\"> \r\n                                            Интернет-магазин Warpoint\r\n                                        </p> \r\n                                    </a> \r\n                                </td> \r\n                            </tr> \r\n                        </tbody> \r\n                    </table> \r\n                </td> \r\n            </tr> \r\n            <tr style=\"height: 300px;\"> \r\n                <td align=\"center\" style=\"border: none; \r\n                           padding-right: 20px;padding-left:20px\"> \r\n  \r\n                    <p style=\"font-weight: bolder;font-size: 42px; \r\n                              letter-spacing: 0.025em; \r\n                              color:#121212;\r\n\t\t\t      background-color: white;\"> \r\n                        Здравствуйте, вы оформили у нас заказ! \r\n                    </p> \r\n                </td> \r\n            </tr> \r\n  \r\n            <tr style=\"display: inline-block;\"> \r\n                <td style=\"height: 150px; \r\n                           padding: 20px; \r\n                           border: none;  \r\n                           background-color: white;\"> \r\n                    \r\n                    <h2 style=\"text-align: left; \r\n                               align-items: center;\"> \r\n                        Информация о вашем заказе:\r\n                   </h2> \r\n                    <p class=\"data\" \r\n                       style=\"text-align: justify-all; \r\n                              align-items: center;  \r\n                              font-size: 15px; \r\n                              padding-bottom: 12px;\r\n\t\t\t\tcolor:#121212;\"> \r\n                       Товар: {titleItems} \r\n\t\t\t<br> Адресс доставки: {deliveryAddress}\r\n\t\t\t<br> Дата доставки: {delivaryDate.ToString()}\r\n                    </p> \r\n\r\n                </td> \r\n            </tr> \r\n        </tbody> \r\n    </table> \r\n</body> ";
            emailMessage.Body = new TextPart("html")
            {
                Text = htmlContent
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync("mylongcode2@yandex.ru", "lupxzsfjnbonzeck");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
