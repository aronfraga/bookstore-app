using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Bookstore.Utility {
    public class EmailSender : IEmailSender {
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            //var emailSend = new MimeMessage();
            //emailSend.From.Add(MailboxAddress.Parse("thisisatest@bookstore.com.ar"));
            //emailSend.To.Add(MailboxAddress.Parse(email));
            //emailSend.Subject = subject;
            //emailSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //using (var emailClient = new SmtpClient()) {
            //    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    emailClient.Authenticate("email", "passs");
            //    emailClient.Send(emailSend);
            //    emailClient.Disconnect(true);
            //}

            return Task.CompletedTask;
        }

    }
}
