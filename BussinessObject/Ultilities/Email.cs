using MailKit.Security;
using MimeKit;

namespace Business.Ultilities
{
    public class Email
    {
        public Email()
        {
        }
        public static async Task<bool> SendEmail(string Email, string Subject, string Html)
        {
            try
            {
                var toEmail = Email;
                string from = "roommanagix@gmail.com";
                string pass = "owda kwow ujga pfjm";
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(from));
                message.Subject = "[RMS] " + Subject;
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = Html
                };
                using MailKit.Net.Smtp.SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, pass);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
