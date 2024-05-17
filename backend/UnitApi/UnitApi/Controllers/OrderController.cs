using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace UnitApi.Controllers
{
    public class OrderController : Controller
    {
        [HttpPost]
        [Route("/order")]
        public async Task<IActionResult> SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация интернет магазина Warpoint", "mylongcode2@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("Эта хуйня работает", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync("mylongcode2@yandex.ru", "");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
            return Ok();
        }
    }
}
