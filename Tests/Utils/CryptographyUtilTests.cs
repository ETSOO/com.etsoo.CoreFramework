using com.etsoo.Utils.Crypto;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Utils
{
    /// <summary>
    /// Cryptography tools tests
    /// </summary>
    [TestFixture]
    public class CryptographyUtilTests
    {
        private readonly string data;
        private readonly string password;

        public CryptographyUtilTests()
        {
            data = "Hello, world!";
            password = "test";
        }

        /// <summary>
        /// AES base64 encryption and decryption test
        /// </summary>
        /// <returns></returns>
        [Test]
        public void AESBase64_EncryptAndDecrypt_Test()
        {
            // Arrange
            var bytes = CryptographyUtils.AESEncrypt(data, password);
            var result = Convert.ToBase64String(bytes);

            // Act
            var decryptedData = CryptographyUtils.AESDecrypt(Convert.FromBase64String(result), password);
            var decriptedString = Encoding.UTF8.GetString(decryptedData);

            // Assert
            Assert.AreEqual(data, decriptedString);
        }

        /// <summary>
        /// AES hexadecimal encryption and decryption test
        /// </summary>
        /// <returns></returns>
        [Test]
        public void AESHex_EncryptAndDecrypt_Test()
        {
            // Arrange
            var result = Convert.ToHexString(CryptographyUtils.AESEncrypt(data, password));

            // Act
            var decryptedData = CryptographyUtils.AESDecrypt(Convert.FromHexString(result), password);
            var decriptedString = Encoding.UTF8.GetString(decryptedData);

            // Assert
            Assert.AreEqual(data, decriptedString);
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
            Assert.AreEqual(6, result.Length);
        }

        /// <summary>
        /// SHA512 to Base64 string test
        /// </summary>
        [Test]
        public async Task HMACSHA512ToBase64_HelloWorldTest()
        {
            // Arrange & act
            var result = await CryptographyUtils.HMACSHA512Async(data, password);

            // Assert
            Assert.AreEqual("DAggap48D+1sf4KP+SrgbWlx4mfP9zeAF//ntpqtGINAed6WKFq4KaVI8KySGF0ER4UvL6QV97uZQRLJ5ga7xw==", Convert.ToBase64String(result));
        }

        /// <summary>
        /// SHA512 to Hexadecimal string test
        /// </summary>
        [Test]
        public async Task HMACSHA512ToHex_HelloWorldTest()
        {
            // Arrange & act
            var result = await CryptographyUtils.HMACSHA512Async(data, password);

            // Assert
            Assert.AreEqual("0c08206a9e3c0fed6c7f828ff92ae06d6971e267cff7378017ffe7b69aad18834079de96285ab829a548f0ac92185d0447852f2fa415f7bb994112c9e606bbc7".ToUpper(), Convert.ToHexString(result));
        }
    }
}
