using com.etsoo.Utils.Crypto;
using System.Text;

namespace Tests.Utils
{
    /// <summary>
    /// Cryptography tools tests
    /// </summary>
    [TestClass]
    public class CryptographyUtilTests
    {
        private readonly string data;
        private readonly string password;

        private static string DoSecretData(string input)
        {
            input = input.Replace("-", "");
            input = (char)((input[0] + input.Last()) / 2) + input[2..];
            return input;
        }

        /*
        private static string UnsealData(string field, string input)
        {
            var bytes = CryptographyUtils.AESDecrypt(input, DoSecretData("30ebc559-4d78-4a27-915b-927107e52fa7"));
            return bytes == null ? throw new ApplicationException(field) : Encoding.UTF8.GetString(bytes);
        }
        */

        public CryptographyUtilTests()
        {
            data = "Hello, world!";
            password = "My password";
        }

        /// <summary>
        /// AES base64 encryption and decryption test
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void AESBase64_EncryptAndDecrypt_Test()
        {
            // Arrange
            var result = CryptographyUtils.AESEncrypt(data, password);

            // Act
            var decryptedData = CryptographyUtils.AESDecrypt(result, password);
            var decriptedString = Encoding.UTF8.GetString(decryptedData!);

            // Assert
            Assert.AreEqual(data, decriptedString);
        }

        /// <summary>
        /// CryptoJs AES testing
        /// https://cryptojs.gitbook.io/docs/
        /// </summary>
        [TestMethod]
        public void CryptoJsAESBase64_EncryptAndDecrypt_Test()
        {
            // Salt: baf491fb6fd3208eb7dff8824af0f35e
            var encrypted = "DigePVkv+01baf491fb6fd3208eb7dff8824af0f35ea61f66e2311be8b8413cf701c6a400d2DXtJlfFvecYlyNuZk5ksAA==";

            var pos = encrypted.IndexOf('+');
            var timestamp = encrypted[..pos];
            var passphrase = password + timestamp;
            passphrase += passphrase.Length;
            var source = encrypted[(pos + 1)..];

            // Act
            var actual = Encoding.UTF8.GetString(CryptographyUtils.AESDecrypt(source, passphrase)!);

            // Assert
            Assert.AreEqual(data, actual);
        }

        /// <summary>
        /// Create random digit string
        /// </summary>
        [TestMethod]
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
        [TestMethod]
        public async Task HMACSHA512ToBase64_HelloWorldTest()
        {
            // Arrange & act
            var result = await CryptographyUtils.HMACSHA512Async(data, password);
            var base64 = Convert.ToBase64String(result);

            // Assert
            Assert.AreEqual("5mzh5YCWP2t2nE2dX38A9gDYApo+GHJIxG3TdqcU9dnPwd65+02igo2x4YLY4FYYJou+wNPihiaDiYSMa4eUmw==", base64);
        }

        /// <summary>
        /// SHA512 to Hexadecimal string test
        /// </summary>
        [TestMethod]
        public async Task HMACSHA512ToHex_HelloWorldTest()
        {
            // Arrange & act
            var result = await CryptographyUtils.HMACSHA512Async(data, password);
            var hex = Convert.ToHexString(result);

            // Assert
            Assert.AreEqual("E66CE1E580963F6B769C4D9D5F7F00F600D8029A3E187248C46DD376A714F5D9CFC1DEB9FB4DA2828DB1E182D8E05618268BBEC0D3E286268389848C6B87949B".ToUpper(), hex);
        }

        /// <summary>
        /// MD5 to Hexadecimal string (X2) test
        /// </summary>
        [TestMethod]
        public async Task MD5_Hex_Test()
        {
            // Arrange & act
            var result = await CryptographyUtils.MD5Async("info@etsoo.com");

            // Assert
            Assert.AreEqual("9c7ce665e6f4f4c807912a7486244c90", Convert.ToHexString(result).ToLower());
        }

        [TestMethod]
        public async Task SHA1Tests()
        {
            // Arrange
            var input = "jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://mp.weixin.qq.com?params=value";

            // Act
            var result = await CryptographyUtils.SHA1Async(input);

            // Assert
            Assert.AreEqual("0f9de62fce790f9a083d5c99e95740ceb90c27ed", Convert.ToHexString(result).ToLower());
        }

        [TestMethod]
        public async Task SHA3_Hex_Test()
        {
            // Arrange & act
            var result = await CryptographyUtils.SHA3Async("test");

            // Assert
            Assert.AreEqual("EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF", Convert.ToHexString(result));
        }
    }
}
