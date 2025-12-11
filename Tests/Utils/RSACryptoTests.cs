using com.etsoo.Utils.Crypto;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Tests.Utils
{
    [TestClass]
    public class RSACryptoTests
    {
        readonly string privateKey;
        readonly string publicKey;

        public RSACryptoTests()
        {
            var rsa = RSA.Create(2048);

            privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
        }

        [TestMethod]
        public void EncryptDecryptTests()
        {
            // Arrange
            var sectionText = $@"{{
                ""RSA"": {{
                    ""PublicKey"": ""{publicKey}"",
                    ""PrivateKey"": ""{privateKey}""
                }}
            }}";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sectionText));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("RSA");

            var crypto = new RSACrypto(section);

            var input = "Hello, Etsoo";

            // Act
            var result = crypto.Encrypt(input);
            var decryptResult = Encoding.UTF8.GetString(crypto.Decrypt(result));

            // Assert
            Assert.AreEqual(input, decryptResult);
        }
    }
}
