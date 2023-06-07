using System.Net.Mail;
using System.Net;

namespace WebsiteMusic.Common
{
    public class MailsUtils
    {
        public static async Task<string> SendGmail(string _from, string _to, string _subject, string _body)
        {
            MailMessage message = new MailMessage(_from, _to, _subject, _body);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            message.IsBodyHtml = true;

            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);

            using var smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("hai0xautrai@gmail.com", "rjhyaccvoetvaeap");

            try
            {
                await smtpClient.SendMailAsync(message);
                return "Gui thanh cong";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Gui khong thanh cong";
            }

        }
    }
}
