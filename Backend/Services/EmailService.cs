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

            emailMessage.From.Add(new MailboxAddress("Администрация сайта portableManager", "portablemanager@sibnet.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using(var client = new SmtpClient())
            {
                client.Connect("smtp.sibnet.ru", 25, false);
                await client.AuthenticateAsync("portablemanager@sibnet.ru", "JLTDvhQ1jLcjnmy6F2pz");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
