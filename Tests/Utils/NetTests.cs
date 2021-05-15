using com.etsoo.Utils.Net.SMS;
using com.etsoo.Utils.Net.SMTP;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using NUnit.Framework;
using Tea;

namespace Tests.Utils
{
    [TestFixture]
    public class NetTests
    {
        [Test]
        public void EmailUtilSend_Test()
        {
            // Arrange
            var client = new SMTPClient(new SMTPClientSettings("smtp.exmail.qq.com", 465, true, "info@etsoo.com", "***"));

            var message = new MimeMessage
            {
                Subject = "Hello from ETSOO",
                Body = new TextPart(TextFormat.Html) { Text = "<h1>Hello, world!</h1>" }
            };
            message.From.Add(MailboxAddress.Parse("info@etsoo.com"));
            message.To.Add(MailboxAddress.Parse("xz@etsoo.com"));

            // Act
            var result = Assert.ThrowsAsync<AuthenticationException>(async () =>
            {
                await client.SendAsync(message);
            });

            // Assert
            Assert.IsTrue(result?.Message.StartsWith("535:"));
        }

        [Test]
        public void AliyunSMSClient_Test()
        {
            // Arrange
            var client = new AliyunSMSClient("id", "password");
            client.AddResource(new SMSResource(Template: "SMS_153055065", Signature: "亿速思维"));

            // Act
            var result = Assert.ThrowsAsync<TeaException>(async () =>
            {
                await client.SendCodeAsync("138***", "123456");
            });

            // Assert
            Assert.AreEqual("InvalidAccessKeyId.NotFound", result?.Code);
        }
    }
}
