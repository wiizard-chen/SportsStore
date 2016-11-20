using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Net.Mail;
using System.Net;
namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "order@example.com";
        public string MailFormAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "MySmtpUsername";
        public string Password = "MySmtpPassword";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\s[orts_store_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder().Append("A new order has been subimitted").AppendLine("---").AppendLine("Items:");
                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal : {2:c}", line.Quantity, line.Product.Name, subtotal);
                }
                body.AppendFormat("Total order value : {0:c}", cart.ComputeTotalValue())
                    .AppendLine("----")
                    .AppendLine("Ship to :")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("----")
                    .AppendFormat("Gift wrap : {0}", shippingDetails.GiftWrap ? "YES" : "NO");
                MailMessage mailmessage = new MailMessage(emailSettings.MailFormAddress, emailSettings.MailToAddress, "New order submitted!", body.ToString());
                if (emailSettings.WriteAsFile)
                    mailmessage.BodyEncoding = Encoding.ASCII;
                try
                {
                    smtpClient.Send(mailmessage);
                }
                catch
                { }
            }
        }
    }
}
