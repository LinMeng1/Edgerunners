using Moon.Core.Models._Imaginary;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Moon.Core.Utilities
{
    public class Mail
    {
        private static string address = "linmeng2@lenovo.com";
        public static void Send(string category, string subject, string context, List<string> receivers, List<string> ccReceivers, List<_Attachment> attachments)
        {
            MailMessage msg = new()
            {
                From = new MailAddress(address, category),
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                Subject = subject,
                Body = context,
            };
            foreach (var receiver in receivers)
            {
                msg.To.Add(receiver);
            }
            foreach (var ccReceiver in ccReceivers)
            {
                msg.CC.Add(ccReceiver);
            }
            foreach (var attachment in attachments)
            {
                byte[] array = Encoding.ASCII.GetBytes(attachment.Base64Str);
                MemoryStream stream = new(array);
                msg.Attachments.Add(new Attachment(stream, attachment.Name));
            }
            SmtpClient smtp = new()
            {
                Host = "10.114.130.182",
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(msg.From.Address, string.Empty)
            };
            smtp.Send(msg);
        }
    }
}
