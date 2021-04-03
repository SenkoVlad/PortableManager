using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace PortableManager.Web.Server.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта portableManager", "vlad.senko2011@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect("in-v3.mailjet.com", 587, false);
                await client.AuthenticateAsync("592f79b28fcb148f26dde9b694a8e845", "293eb295472b602164bc130e70721344");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
