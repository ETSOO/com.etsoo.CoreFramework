using com.etsoo.SMTP;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace Tests.Utils
{
    [TestClass]
    public class NetTests
    {
        MimeMessage message => new MimeMessage
        {
            Subject = "Hello from ETSOO",
            Body = new TextPart(TextFormat.Html) { Text = "<h1>Hello, world!</h1>" },
            To = { MailboxAddress.Parse("xz@etsoo.com") }
        };

        [TestMethod]
        public async Task EmailUtilSend_Test()
        {
            // Arrange
            var client = new SMTPClient(new SMTPClientOptions("smtp.exmail.qq.com", 465, true, "ETSOO <info@etsoo.com>", "info@etsoo.com", "***"));

            // Act
            var result = await Assert.ThrowsAsync<AuthenticationException>(async () => await client.SendAsync(message));

            // Assert
            Assert.IsTrue(result?.Message.StartsWith("535:") ?? false);
        }

        [TestMethod]
        public async Task SMTPClient_ConfigurationInit_Tests()
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
            var client = new SMTPClient(section.Get<SMTPClientOptions>()!);

            // Act
            var result = await Assert.ThrowsAsync<AuthenticationException>(async () => await client.SendAsync(message));

            // Assert
            Assert.IsTrue(result?.Message.StartsWith("535:") ?? false);
        }
    }
}
