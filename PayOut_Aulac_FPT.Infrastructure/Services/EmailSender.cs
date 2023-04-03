using PayOut_Aulac_FPT.Core.Utils.Models;
using System.Net;
using System.Net.Mail;

namespace PayOut_Aulac_FPT.Infrastructure.Services
{
    public class EmailSender
    {
        private EmailConfigInfo _emailConfigInfo;

        public EmailSender(EmailConfigInfo emailConfigInfo)
        {
            _emailConfigInfo = emailConfigInfo;
        }
        public void Send(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient("smtp." + _emailConfigInfo.Host, _emailConfigInfo.Port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential(_emailConfigInfo.Username, _emailConfigInfo.Password);

                // A client has been created, now you need to create a MailMessage object
                //string fromEmail = String.Format("{0}@{1}", _emailConfigInfo.Username, _emailConfigInfo.Host);
                MailMessage message = new MailMessage(
                                         from: _emailConfigInfo.Username ?? "", // From field
                                         toEmail, // Recipient field
                                         subject, // Subject of the email message
                                         body // Email message body
                                      );

                // Send the message
                client.Send(message);
            }
        }
    }

}
