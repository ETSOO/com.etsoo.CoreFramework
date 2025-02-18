using com.etsoo.CoreFramework.Authentication;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Utils;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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
        public void SignDataTest()
        {
            var rsa = new RSACrypto(null, "Pkcs8:MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCNgoWeFN2Wi/fELIPZBV2hWPOm0Fb8621ptyBWfNLFlT553ZH6xC3sZxp9btM7JdwfWvs5VRonpBvfdgIBikMMbp1vCZJONjfpTLNcHlHgdF9rwk4MgCadarBKsMQAfwij8f15O9UVXBhvztGf6Ho09DnitJ1YnUk3kt8ClsLWOT0h3QTwnbjyhpg3/JmJw8jIPdzxxhrmYvqq4qvXzj9H5opjWd9oaMFaiGB4zFBsHMJXuH9ACsbbf6qnzj9AEy6V8cM9VZaTID4nFn5C49z3piawV+yM4P4p4a2SJq/d7dwbzSomPzUvLMnktTvk+NJRyNxyspjqOt72Y4j7YNQDAgMBAAECggEAAMtq1KhpVh8TFRbq5p0RGYbWV2l0E5d+1ckhdVreFB3ya9zCpRXU7C7oByxII1zjD4oDPx8rNm3Feku/VyLfnYJBgA4dtDK5vaWgnDPPYeNFZeWBarCNhvTCaKj1cMtF0SXatoOPfr81o+sVYkB77zAv4wYAnC7F6nn4ppsSHQHTG9nuLP4G0y2ULk9/9q2v/GdynH1OD7P1yUzB15if5IF6zvhwaXUM1PineRf4kYmaHtJdShKAWfVSBvC1Vilk+vqLMbwO55eoBTIHYpsZWln/ec6RE7z+Pb7g0rQdVSKr8BgXt5As5Gh/Y1d4SLXLUA0Zr7AhRpFWspWuvdhzAQKBgQDfS/b8h51xOyrMOPXcel0i3ZyKVJs4mBRVrLr0V1HS3yfAnp/kjC+eK2WTHjGGEPwZtcewtIWXXB+Y5GxpTxhx/53EER8vKH5QND6k5jxtdDQIHwUU5zVIxzyhY0Str6cilwKlWGivM8Io7zT3rHn1R87yLNyan2uj3OPztmJRiQKBgQCiPCHXEplzZmHO2xMRzkCe5XV8ZZLZQcVCuZpmj3c2wNWGrDTYJlYnV9dc1W0cyOwG8kyOPcC3ho9iRwth25nxMnw5afMtOYvwI4EvLmd22iiAGEAj9O0dOCB6Yxmis9asdttYhZCkL1G6HUCG4Mn6XSrLHZ2ruEuRae32/TWSKwKBgCaCDTgDky1BzOGnOQ8qswEeQq7AZHxgDbGwthUJMf0xqsNXF6/sVRHr3fp/DH9YUoGEjcl1eExgALr3OZL3pvmR4X08jqotS4s9V0hMxEMD9S0pXFD8hn3kjhou6lshnasja7tkAbmlLWitx+6meenI1nGBNxIbSA7cOxt+anoxAoGAed6wEQ9AxKahTLHXNmX4tyRpyCPJV3kHxOMGMIsPI8th24PbQpAx4eYjuvH8wEXSwDkd9zA+Z98mMM5rp3w+vSiOltaXPV7gV2lkbtvuDyM8j1UoQZqI0I1MIIP3SvjLh8zVYz8ac6u0OholUezk7TU1o1VBDzEnWzn277YwmvcCgYEAsHp6kOcKNxAvfzE8XZ0RfFNlfRREGtmJOsiNWqJzpyKRTSGsxRQUiNnd1rEWXGD0dBI1//d23//9FJA9uS7A4qKjo/EnuH05pjsTaB3zVyirtYcFKJL3DgbMeFoCKGUUbUmAEe7vIxdAKbpjJRL4HHZwwb8BcG7bHLmYFdbya5U=");
            var sourceBytes = Encoding.UTF8.GetBytes("app_id=2021004153681483&charset=UTF-8&format=json&method=alipay.system.oauth.token&sign_type=RSA2&timestamp=2024-07-01 07:25:33&version=1.0");
            var bytes = rsa.SignData(sourceBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var sign = Convert.ToBase64String(bytes);
            Assert.That(sign, Is.EqualTo("UItF4asgd6OX+6uwjJr33KEA+FnyO00KeFaxNZOlEGE8BTN9nQbG0DJxTnwFonF7qHd9bBVptvTZCsSHrS5aww5n1ofPCuadewVJ14LJyexIURF6Rz8TDadXQlfDsCG7DrfIzs4ixeybwbPpNZMTFn1lpUmqR5jfybcR/aXT1RW9kh8PK6Q6m5Kp2Pl2q13DRGX7hCyJFvkq0ZdwRMtAgQAOCY5U9DYDROKVsAr+A/TyIQTQFIcIFtej+kDHZ5SKFllXgmPQONA3nEDOj/bU758rDnaQ3MKBPXe7FPUJooPtMTvAOD6fgnuOEXh9vStqOQKhuBeWAsnBxOP1L6uSWQ=="));
        }

        [Test]
        public void CreateAccessToken_Tests()
        {
            // Arrange
            string[] userScopes = ["core", "crm"];
            var user = new CurrentUser
            {
                Id = "1",
                Scopes = userScopes,
                Name = "Etsoo User",
                RoleValue = 1,
                ClientIp = IPAddress.Parse("127.0.0.1"),
                Region = "CN",
                Organization = "0",
                Oid = "0",
                DeviceId = "1",
                App = "1",
                Language = CultureInfo.CurrentCulture
            };

            // Json Data
            user.JsonData.Add("CustomAmount", 12);
            user.JsonData.Add("CustomFlag", false);

            // Act
            var token = service.CreateAccessToken(user);

            // Serialization
            var userJson = JsonSerializer.Serialize(user, ModelJsonSerializerContext.Default.CurrentUser);

            // Deserialize
            var user2 = JsonSerializer.Deserialize(userJson, ModelJsonSerializerContext.Default.CurrentUser);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(token, Is.Not.Null);
                Assert.That(user2, Is.Not.Null);
                Assert.That(user2!.ClientIp, Is.EqualTo(user.ClientIp));
                Assert.That(user2.Language, Is.EqualTo(user.Language));
                Assert.That(user2.AppId, Is.EqualTo(user.AppId));
                Assert.That(userJson, Does.Contain("customAmount"));
                Assert.That(user2.JsonData.Get<bool>("CustomFlag"), Is.False);
            });

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

            // Validate refresh token
            var (claimsPrincipal, securityToken) = publicService.ValidateToken(token);

            var scopes = claimsPrincipal?.Claims.Where(claim => claim.Type == "scope").Select(claim => claim.Value);

            Assert.Multiple(() =>
            {
                Assert.That(claimsPrincipal, Is.Not.Null);
                Assert.That(claimsPrincipal?.Claims.GetValue(UserToken.IPAddressClaim), Is.EqualTo("127.0.0.1"));
                Assert.That(scopes, Is.EqualTo(userScopes));
            });

            // Public service should not generate token
            Assert.Throws<InvalidOperationException>(() =>
            {
                publicService.CreateAccessToken(user);
            });
        }

        [Test]
        public void CreateIdToken_Tests()
        {
            // Arrange
            var userName = "Etsoo User";
            var app = "#xaa";
            string[] userScopes = ["core", "crm"];
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            var jsonData = new StringKeyDictionaryObject
            {
                { "CustomAmount", 12 }
            };
            var user = new CurrentUser
            {
                Id = "1",
                Scopes = userScopes,
                Name = userName,
                RoleValue = 1,
                ClientIp = IPAddress.Parse("127.0.0.1"),
                Region = "CN",
                Organization = "0",
                Oid = "0",
                DeviceId = "1",
                Language = CultureInfo.CurrentCulture,
                App = app,
                TimeZone = timezone,
                JsonData = jsonData
            };

            // Act
            var identity = user.CreateIdentity();
            var signingKey = new string('-', 32);
            var token = service.CreateIdToken(identity, signingKey, "app6");

            // Arrange, public key verification
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JwtText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");
            var publicService = new JwtService(new ServiceCollection(), section.Get<JwtSettings>(), null);

            // Validate id token
            var (cp, securityToken) = publicService.ValidateIdToken(token, signingKey, null, "app6");
            var jwt = securityToken as JwtSecurityToken;
            var claims = jwt?.Claims;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(identity.IsAuthenticated, Is.False);
                Assert.That(cp?.Identity?.IsAuthenticated, Is.True);
                Assert.That(identity.Name, Is.EqualTo(user.Id));
                Assert.That(claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iss)?.Value, Is.EqualTo("Etsoo"));
                Assert.That(claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value, Is.EqualTo(userName));
                Assert.That(claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value, Is.EqualTo("1"));
                Assert.That(claims?.FirstOrDefault(c => c.Type == CurrentUser.AppClaim)?.Value, Is.EqualTo(app));
                Assert.That(claims?.FirstOrDefault(c => c.Type == UserToken.TimeZoneClaim)?.Value, Is.EqualTo(timezone.Id));
                Assert.That(claims?.FirstOrDefault(c => c.Type == MinUserToken.JsonDataClaim)?.Value, Is.EqualTo("{\"customAmount\":12}"));
            });
        }

        [Test]
        public void ValidateIdToken_Tests()
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JwtText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");
            var publicService = new JwtService(new ServiceCollection(), section.Get<JwtSettings>(), null);

            var (cp, token) = publicService.ValidateIdToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxMDEwIiwic2NvcGUiOiJjb3JlIiwicmVnaW9uIjoiQ04iLCJpcGFkZHJlc3MiOiI6OjEiLCJkZXZpY2VpZCI6IjEwMDYiLCJvcmdhbml6YXRpb24iOiIwIiwibmFtZSI6IuiClui1niIsImxvY2FsaXR5IjoiemgtQ04iLCJyb2xlIjoiMTYiLCJvaWQiOiIwIiwibmJmIjoxNzI3MTQ4NDA2LCJleHAiOjE3MjcxNDg3MDYsImlhdCI6MTcyNzE0ODQwNiwiaXNzIjoiU21hcnRFUlAiLCJhdWQiOiJBTEwifQ.2zODXPP5NMz_zMKOoWeydzkbZUizFyFrkbvWAZVc2_g", "JwANgd$v=U*cW9-Dg7DA=jejn2UN<t-S", null, null, false);

            var user = CurrentUser.Create(cp, out _);

            Assert.Multiple(() =>
            {
                Assert.That(user, Is.Not.Null);
                Assert.That(token, Is.Not.Null);
                Assert.That(token?.Issuer, Is.EqualTo("SmartERP"));
                Assert.That(user?.Id, Is.EqualTo("1010"));
                Assert.That(user?.Organization, Is.EqualTo("0"));
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
