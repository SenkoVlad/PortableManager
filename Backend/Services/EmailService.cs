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

            emailMessage.From.Add(new MailboxAddress("Администрация сайта portableManager", "portablemanager@rambler.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using(var client = new SmtpClient())
            {
                client.Connect("smtp.rambler.ru", 465, true);
                await client.AuthenticateAsync("portablemanager@rambler.ru", "JLTDvhQ1jLcjnmy6F2pz");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
