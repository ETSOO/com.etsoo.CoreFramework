using com.etsoo.Utils.Crypto;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests.Utils
{
    /// <summary>
    /// Cryptography tools tests
    /// </summary>
    [TestFixture]
    public class CryptographyUtilTests
    {
        private readonly ReadOnlyMemory<char> data;
        private readonly ReadOnlyMemory<char> password;

        public CryptographyUtilTests()
        {
            data = "Hello, world!".AsMemory();
            password = "test".AsMemory();
        }

        /// <summary>
        /// AES base64 encryption and decryption test
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AESBase64_EncryptAndDecrypt_Test()
        {
            // Arrange
            var result = await CryptographyUtils.AESEncryptToBase64Async(data, password);

            // Act
            var decryptedData = await CryptographyUtils.AESDecryptFromBase64Async(result, password);
            var decriptedString = decryptedData.Span.ToString();

            // Assert
            Assert.IsTrue(decriptedString.Equals(data.Span.ToString()));
        }

        /// <summary>
        /// AES hexadecimal encryption and decryption test
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AESHex_EncryptAndDecrypt_Test()
        {
            // Arrange
            var result = await CryptographyUtils.AESEncryptToHexAsync(data, password);

            // Act
            var decryptedData = await CryptographyUtils.AESDecryptFromHexAsync(result, password);
            var decriptedString = decryptedData.Span.ToString();

            // Assert
            Assert.IsTrue(decriptedString.Equals(data.Span.ToString()));
        }

        /// <summary>
        /// Create random digit string
        /// </summary>
        [Test]
        public void CreateRandString_Digit_Test()
        {
            // Arrange & act
            var result = CryptographyUtils.CreateRandString(RandStringKind.Digit, 6);

            // Assert
            Assert.IsTrue(result.Length == 6);
        }

        /// <summary>
        /// SHA512 test
        /// </summary>
        [Test]
        public void HMACSHA512_HelloWorldTest()
        {
            // Arrange & act
            var result = CryptographyUtils.HMACSHA512(data.Span, password.Span);

            // Assert
            Assert.IsTrue(Convert.ToBase64String(result) == "DAggap48D+1sf4KP+SrgbWlx4mfP9zeAF//ntpqtGINAed6WKFq4KaVI8KySGF0ER4UvL6QV97uZQRLJ5ga7xw==");
        }

        /// <summary>
        /// SHA512 to Base64 string test
        /// </summary>
        [Test]
        public async Task HMACSHA512ToBase64_HelloWorldTest()
        {
            // Arrange & act
            var result = await CryptographyUtils.HMACSHA512ToBase64Async(data, password);

            // Assert
            Assert.IsTrue(result.ToString() == "DAggap48D+1sf4KP+SrgbWlx4mfP9zeAF//ntpqtGINAed6WKFq4KaVI8KySGF0ER4UvL6QV97uZQRLJ5ga7xw==");
        }

        /// <summary>
        /// SHA512 to Hexadecimal string test
        /// </summary>
        [Test]
        public async Task HMACSHA512ToHex_HelloWorldTest()
        {
            // Arrange & act
            var result = await CryptographyUtils.HMACSHA512ToHexAsync(data, password);

            // Assert
            Assert.IsTrue(result.ToString().Equals("0c08206a9e3c0fed6c7f828ff92ae06d6971e267cff7378017ffe7b69aad18834079de96285ab829a548f0ac92185d0447852f2fa415f7bb994112c9e606bbc7", StringComparison.OrdinalIgnoreCase));
        }
    }
}
