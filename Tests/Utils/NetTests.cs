using com.etsoo.Utils.Net.SMTP;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using NUnit.Framework;

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
    }
}
