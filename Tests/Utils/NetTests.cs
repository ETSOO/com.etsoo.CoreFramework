using com.etsoo.Utils.Net.SMTP;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using NUnit.Framework;
using System.Text;

namespace Tests.Utils
{
    [TestFixture]
    public class NetTests
    {
        readonly MimeMessage message;

        public NetTests()
        {
            message = new MimeMessage
            {
                Subject = "Hello from ETSOO",
                Body = new TextPart(TextFormat.Html) { Text = "<h1>Hello, world!</h1>" }
            };
            message.To.Add(MailboxAddress.Parse("xz@etsoo.com"));
        }

        [Test]
        public void EmailUtilSend_Test()
        {
            // Arrange
            var client = new SMTPClient(new SMTPClientSettings("smtp.exmail.qq.com", 465, true, "ETSOO <info@etsoo.com>", "info@etsoo.com", "***"));

            // Act
            var result = Assert.ThrowsAsync<AuthenticationException>(async () =>
            {
                await client.SendAsync(message);
            });

            // Assert
            Assert.IsTrue(result?.Message.StartsWith("535:"));
        }

        [Test]
        public void SMTPClient_ConfigurationInit_Tests()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"{
                ""SMTP"": {
                    ""Host"": ""smtp.exmail.qq.com"",
                    ""Port"": 465,
                    ""UseSsl"": true,
                    ""UserName"": ""info@etsoo.com"",
                    ""Password"": ""***""
                }
            }"));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("SMTP");

            var client = new SMTPClient(section);

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
