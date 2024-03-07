using com.etsoo.CoreFramework.Authentication;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Globalization;
using System.Net;
using System.Text;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class JwtServiceTests
    {
        readonly JwtService service;

        readonly string JwtText = @"{
                ""Jwt"": {
                    ""DefaultIssuer"": ""Etsoo"",
                    ""Audience"": ""all"",
                    ""EncryptionKey"": ""a#o249YjbV>2i7Bl*xXeGa)veKs9VXu*"",
                    ""PublicKey"": ""MIIBCgKCAQEAxGlFGuz35fhYvlTaRAJ5eVCKOp04nijVdtBVj9z/czWiYzAmyLwYGMhX7Sf1SeVnzHPE8TPpuaHDrVofdieB/u7RwEgWATqkX0Uneimye+uUeLB8X3yAj2rArgvAwYSCQ6DpAAF/SR/OrwWpc8dsx5JcaAkJ7XT5zrNXez+TSyokSACsii1M8gUQ5xfY7eFnPi7KTg5JGG9x6idq0P4/eSR8RbIo5n+fAoB5loME7EcRIf5aXsSNLMaGkTSe29MOobFoD8SjH6drWOk2TCCFhpELWMDVe1wYXmPMiA+K4OePe3gl3dc6DWIjvSZVAB9yO8L5Mvp7HS0iVp5YuzZrUQIDAQAB""
                }
            }";

        readonly string JwtTextPrivate = @"{
                ""Jwt"": {
                    ""DefaultIssuer"": ""Etsoo"",
                    ""ValidIssuers"": [ ""S2"", ""Etsoo"" ],
                    ""Audience"": ""EtsooRereshToken"",
                    ""EncryptionKey"": ""a#o249YjbV>2i7Bl*xXeGa)veKs9VXu*"",
                    ""PrivateKey"": ""MIIEpAIBAAKCAQEAxGlFGuz35fhYvlTaRAJ5eVCKOp04nijVdtBVj9z/czWiYzAmyLwYGMhX7Sf1SeVnzHPE8TPpuaHDrVofdieB/u7RwEgWATqkX0Uneimye+uUeLB8X3yAj2rArgvAwYSCQ6DpAAF/SR/OrwWpc8dsx5JcaAkJ7XT5zrNXez+TSyokSACsii1M8gUQ5xfY7eFnPi7KTg5JGG9x6idq0P4/eSR8RbIo5n+fAoB5loME7EcRIf5aXsSNLMaGkTSe29MOobFoD8SjH6drWOk2TCCFhpELWMDVe1wYXmPMiA+K4OePe3gl3dc6DWIjvSZVAB9yO8L5Mvp7HS0iVp5YuzZrUQIDAQABAoIBADhYftOvoZpeuY64pvkVwKV13oHcMq7kxgBU2gbwfnQdsd1Epgu2Mi+B4f+OFAdEAZgcqiYMH4P6jTlA/n+V6+wntRK1W2K04QzXuPCJ38M/HRPWhYebHwKFvIrxojWh/wDJu64dv9dJgbCiLi+hyWojadEKrsdpZHTQ7ErWPTo1dtYJ3FEI7tPHfCh7SF8AAR3eGoizHlw5is8Lm1+FZ+CjlZdO8bkIdq6dRfggsE/UnePekR42I352Lt+gzRkZvsyLYxb8QpSePwhay0f+M/DzOCtdIdpaOo2TbmVl5Lp9C7yCvl9HlJBZOAiLUlGpGZl3uCBDERPhuBbpvpQLoXUCgYEA0s8Q3wg6NaRwErxAJIhijsmAFtApgMO3m6HY5OTPtYOSX0rUywmj0Ze8WZntKaP9YuEm2LNUOhWOycCdxwEzqBw2v1A5GpYBosjrne7SO8wjJtzVA2lD53D0mfQJk7wFQnQtXe+jQvVPvXk8M/69Ehyyf9fKmQFxpADMPO5enfMCgYEA7oQUOLXMCpbf0LP3YNDXXQ2TqtWg0qJAKbcR1GDBAQUBXmvb7EMkRbBRmpwM7NOjUhB4c4n641Vk5fPDuenAo6QZgwok29gGfbuUJ9CoD6cUDPGaS/9+V4TLGCa8JlU9EGax6ZN28ib8E8D2ONkIkoxioG9rEZNQQH658lPp7qsCgYEAnywSBRVlPlOm+76AgBUqtb2XpaIPdFZTMIQIDOxnmRp7TtBl09i3hO4ZHV6IIETeceanOkBNfH4CjnuNplFV+70x6Updk6FoIs2qell1DAmbESD1BdpZl0tGpWgKQy5YmlC2YJMspsYrK1l7B5d0k1Rvwu/g3Z6le7vOesNRVdkCgYEA6F509OroZkimtEUgdIoBxv47JuwJSiwYJovcKvQ4FC40LzEViQ/AfsRQPDhbvz5QltrE/osmiePf8MeTn8RDkSmGUUvIrQXBDS16IW/+y4ES36lhkTjQdaNB2b2jABV68EecfNrVvwSMARK0zp1i5KMwUu05el3yiWLs9VEbTxECgYA5jdWueGIsK0OYKgvfyjLmPxqzdAMNK2UvXQWQn74YRfULLeYebi/jXbLD2WHIIhrsGcSBqgnoabbTn9a7WYAMdcvFuXiKO3TpXbRW0qNo705YdV/Ot2WriVrSjfjuYLUT14MaAxfT0FR4DXmTkhgCDDhF5cOWdKUgdXmHZoWKMg=="",
                    ""PublicKey"": ""MIIBCgKCAQEAxGlFGuz35fhYvlTaRAJ5eVCKOp04nijVdtBVj9z/czWiYzAmyLwYGMhX7Sf1SeVnzHPE8TPpuaHDrVofdieB/u7RwEgWATqkX0Uneimye+uUeLB8X3yAj2rArgvAwYSCQ6DpAAF/SR/OrwWpc8dsx5JcaAkJ7XT5zrNXez+TSyokSACsii1M8gUQ5xfY7eFnPi7KTg5JGG9x6idq0P4/eSR8RbIo5n+fAoB5loME7EcRIf5aXsSNLMaGkTSe29MOobFoD8SjH6drWOk2TCCFhpELWMDVe1wYXmPMiA+K4OePe3gl3dc6DWIjvSZVAB9yO8L5Mvp7HS0iVp5YuzZrUQIDAQAB""
                }
            }";

        public JwtServiceTests()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JwtTextPrivate));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");

            service = new JwtService(new ServiceCollection(), section.Get<JwtSettings>(), null);
        }

        [Test]
        public void CreateAccessToken_Tests()
        {
            // Arrange
            var user = new CurrentUser("1", null, null, "Etsoo", 1, IPAddress.Parse("127.0.0.1"), 1, CultureInfo.CurrentCulture, "CN") { JsonData = "{ body: \"In this scenario, the external client will give you the structure of JWT, normally with custom claims that they expect and provide you with an RSA private key to sign the token. The token will then be used to construct a Uri that will be sent to users and allowing them to invoke the external client endpoints.\" }" };

            // Act
            var token = service.CreateAccessToken(user);

            // Assert
            Assert.That(token, Is.Not.Null);

            // Arrange, public key verification
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JwtText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");

            /**
             * , (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
            {
                return new List<RsaSecurityKey> { };
            }
             */
            var publicService = new JwtService(new ServiceCollection(), section.Get<JwtSettings>(), null);

            // Refresh token
            var refreshToken = service.CreateRefreshToken(new RefreshToken("1", null, IPAddress.Parse("127.0.0.1"), "CN", 1, "service"));

            // Validate refresh token
            var (claimsPrincipal, expired, kid, securityToken) = publicService.ValidateToken(refreshToken);

            Assert.Multiple(() =>
            {
                Assert.That(expired, Is.False);
                Assert.That(claimsPrincipal, Is.Not.Null);
                Assert.That(kid, Is.EqualTo("service"));
            });

            // Public service should not generate token
            Assert.Throws<InvalidOperationException>(() =>
            {
                publicService.CreateAccessToken(user);
            });
        }

        [Test]
        public void SignDataAndVerify_Tests()
        {
            // Arrange, public key service
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JwtText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");

            var publicService = new JwtService(new ServiceCollection(), section.Get<JwtSettings>());

            var rawData = "In this scenario, the external client will give you the structure of JWT, normally with custom claims that they expect and provide you with an RSA private key to sign the token. The token will then be used to construct a Uri that will be sent to users and allowing them to invoke the external client endpoints.";
            var signResult = service.SignData(rawData);
            var result = publicService.VerifyData(Encoding.UTF8.GetBytes(rawData), signResult);
            Assert.That(result);
        }
    }
}
