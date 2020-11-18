using com.etsoo.CoreFramework.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Utils
{
    /// <summary>
    /// Cryptography tools tests
    /// </summary>
    [TestClass]
    public class CryptographyUtilTests
    {
        /// <summary>
        /// SHA512 to Base64 string test
        /// </summary>
        [TestMethod]
        public void HMACSHA512ToBase64_HelloWorldTest()
        {
            // Arrange & act
            var result = CryptographyUtil.HMACSHA512ToBase64("Hello, world!", "test");

            // Assert
            Assert.IsTrue(result == "DAggap48D+1sf4KP+SrgbWlx4mfP9zeAF//ntpqtGINAed6WKFq4KaVI8KySGF0ER4UvL6QV97uZQRLJ5ga7xw==");
        }

        /// <summary>
        /// SHA512 to Hexadecimal string test
        /// </summary>
        [TestMethod]
        public void HMACSHA512ToHex_HelloWorldTest()
        {
            // Arrange & act
            var result = CryptographyUtil.HMACSHA512ToHex("Hello, world!", "test");

            // Assert
            Assert.IsTrue(result == "0c08206a9e3c0fed6c7f828ff92ae06d6971e267cff7378017ffe7b69aad18834079de96285ab829a548f0ac92185d0447852f2fa415f7bb994112c9e606bbc7");
        }
    }
}
