using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Authentication;
using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class JwtServiceTests
    {
        readonly JwtService service;

        public JwtServiceTests()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"{
                ""Jwt"": {
                    ""Issuer"": ""etsoo"",
                    ""Audience"": ""all""
                }
            }"));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");

            service = new JwtService(new ServiceCollection(), false, section, Encoding.UTF8.GetBytes("SecurityKeyShouldBeLongerThan128"));
        }

        [Test]
        public void CreateAccessToken_Tests()
        {
            // Arrange
            var user = new CurrentUser("1", "Etsoo", new string[] { "Admin" }, IPAddress.Parse("127.0.0.1"), CultureInfo.CurrentCulture, null);

            // Act
            var token = service.CreateAccessToken(user);

            var t = service.CreateRefreshToken(user);

            // Assert
            Assert.AreEqual(3, token.Split('.').Length);
        }

        [Test]
        public void ValidateToken_Tests()
        {
            // Arrange
            var token = "1eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkV0c29vIiwibmFtZWlkIjoiMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2xvY2FsaXR5IjoiemgtQ04iLCJyb2xlIjoiQWRtaW4iLCJpcGFkZHJlc3MiOiIxMjcuMC4wLjEiLCJuYmYiOjE2MjI3MTk1OTgsImV4cCI6MTYyNDAxNTU5OCwiaWF0IjoxNjIyNzE5NTk4LCJpc3MiOiJldHNvbyIsImF1ZCI6IlJlZnJlc2hUb2tlbiJ9.Wdp-CQ-8R6c_s80IW8vkeDSYuxZ57Ue18_XDeoB-10EsMzbl2wIN8bTF2a8q1qTcB8nGNTj1NsChS4mrqNwcpw";

            // Act
            var result = service.ValidateRefreshToken(token);

            // Assert
            Assert.IsNull(result);
        }
    }
}
