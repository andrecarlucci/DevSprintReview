using System.Net;
using System.Net.Mail;

namespace DevSprintReview.Mails {
    public static class EmailSender {

        private static MailAddress _fromAddress = new MailAddress("andre.carlucci@way2.com.br", "André Carlucci");
        private static string _fromPassword = "kgirnylawguktelp";

        public static void Send(string emailTo, string nameTo, string subject, string body) {
            var toAddress = new MailAddress(emailTo, nameTo);

            using (var smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromAddress.Address, _fromPassword),
                Timeout = 20000
            }) {
                using (var message = new MailMessage(_fromAddress, toAddress) {
                    Subject = subject,
                    Body = body
                }) {
                    smtp.Send(message);
                }
            }
        }
    }
}