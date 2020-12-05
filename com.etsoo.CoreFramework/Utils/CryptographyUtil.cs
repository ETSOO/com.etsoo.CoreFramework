using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Utils
{
    /// <summary>
    /// Cryptography Tools
    /// 密码工具
    /// </summary>
    public static class CryptographyUtil
    {
        private static AesManaged AESManagedCreate(string passPhrase)
        {
            // Random byte
            // 随机字节
            var randByte = (byte)(passPhrase.Length % 7);

            // Salt
            // 调味数
            var salt = new byte[] { 0, 1, 2, randByte, 4, 5, 6, 7 };

            // Hash the user password along with the salt
            // 由密匙创建加密的键，而不要直接使用密匙
            // https://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            using var password = new Rfc2898DeriveBytes(passPhrase, salt);

            //  Clear salt bytes
            Array.Clear(salt, 0, salt.Length);
            salt = null;

            // 32 x 8 = 256 bits
            // 16 x 8 = 128 bits
            return new AesManaged()
            {
                Key = password.GetBytes(32),
                IV = password.GetBytes(16)
            };
        }

        /// <summary>
        /// AES 256-bit symmetric decryption
        /// AES 256位对称解密
        /// </summary>
        /// <param name="cipherTextBytes">Cipher text bytes</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<string> AESDecryptAsync(byte[] cipherTextBytes, string passPhrase)
        {
            using var aesAlg = AESManagedCreate(passPhrase);
            using var decryptor = aesAlg.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipherTextBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return await srDecrypt.ReadToEndAsync();
        }

        /// <summary>
        /// AES 256-bit symmetric decryption from base64 string
        /// AES 256位对称解密，从Base64字符串
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<string> AESDecryptFromBase64Async(string cipherText, string passPhrase)
        {
            return await AESDecryptAsync(FromBase64String(cipherText), passPhrase);
        }

        /// <summary>
        /// AES 256-bit symmetric decryption from hexadecimal string
        /// AES 256位对称解密，从16进制字符串
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<string> AESDecryptFromHexAsync(string cipherText, string passPhrase)
        {
            return await AESDecryptAsync(FromHexadecimalString(cipherText), passPhrase);
        }

        /// <summary>
        /// AES 256-bit symmetric encryption
        /// AES 256位对称加密
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<byte[]> AESEncryptAsync(string plainText, string passPhrase)
        {
            using var aesAlg = AESManagedCreate(passPhrase);
            using var encryptor = aesAlg.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);

            // Write the plain text for encryption
            await swEncrypt.WriteAsync(plainText);

            // Close the writer then flush the data to MemoryStream
            await swEncrypt.DisposeAsync();

            // Return byte array
            return msEncrypt.ToArray();
        }

        /// <summary>
        /// AES 256-bit symmetric encryption, to base64 string
        /// AES 256位对称加密，到Base64字符串
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted base64 string</returns>
        public static async Task<string> AESEncryptToBase64Async(string plainText, string passPhrase)
        {
            return ToBase64String(await AESEncryptAsync(plainText, passPhrase));
        }

        /// <summary>
        /// AES 256-bit symmetric encryption, to Hexadecimal string
        /// AES 256位对称加密，到16进制字符串
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted hexadecimal string</returns>
        public static async Task<string> AESEncryptToHexAsync(string plainText, string passPhrase)
        {
            return ToHexadecimalString(await AESEncryptAsync(plainText, passPhrase));
        }

        /// <summary>
        /// Create random bytes
        /// </summary>
        /// <param name="size">Size</param>
        /// <returns>Random bytes</returns>
        public static byte[] CreateRandBytes(int size)
        {
            // Init
            var bytes = new byte[size];

            // RNGCryptoServiceProvider is essential
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(bytes);
            }

            // Return
            return bytes;
        }

        /// <summary>
        /// Create random string
        /// 创建随机字符串
        /// </summary>
        /// <param name="kind">类型</param>
        /// <param name="size">长度</param>
        /// <returns>随机字符串</returns>
        public static string CreateRandString(RandStringKind kind, int size)
        {
            // Collection
            var collection = new StringBuilder();

            if (kind.HasFlag(RandStringKind.Digit))
            {
                collection.Append("0123456789");
            }

            if (kind.HasFlag(RandStringKind.LowerCaseLetter))
            {
                collection.Append("abcdefghijklmnopqrstuvwxyz");
            }

            if (kind.HasFlag(RandStringKind.UperCaseLetter))
            {
                collection.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }

            if (kind.HasFlag(RandStringKind.Symbol))
            {
                collection.Append("~!@#$%^&*()-=+:<>");
            }

            // Target random bytes
            var byes = CreateRandBytes(size);

            // Length
            var len = collection.Length;

            var result = new StringBuilder(size);

            foreach (byte b in byes)
            {
                result.Append(collection[b % len]);
            }

            // Return
            return result.ToString();
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512
        /// 基于哈希的消息认证码（HMAC）, SHA512
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed bytes</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<byte[]> HMACSHA512Async(string message, string privateKey)
        {
            // HMAC
            using var alg = new HMACSHA512(Encoding.UTF8.GetBytes(privateKey));

            alg.Initialize();

            return await alg.ComputeHashAsync(new MemoryStream(Encoding.UTF8.GetBytes(message)));
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512 to Base64 string
        /// 基于哈希的消息身份验证代码（HMAC）, SHA512到Base64字符串
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed message</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<string> HMACSHA512ToBase64Async(string message, string privateKey)
        {
            return ToBase64String(await HMACSHA512Async(message, privateKey));
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512 to Hexadecimal string
        /// 基于哈希的消息身份验证代码（HMAC）, SHA512到16进制字符串
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed message</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<string> HMACSHA512ToHexAsync(string message, string privateKey)
        {
            return ToHexadecimalString(await HMACSHA512Async(message, privateKey));
        }

        /// <summary>
        /// Convert base64 string to bytes
        /// Base64字符串转化字节数组
        /// </summary>
        /// <param name="base64String">Base64 string</param>
        /// <returns>Bytes</returns>
        public static byte[] FromBase64String(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        /// <summary>
        /// Convert bytes to base64 string
        /// 转化字节数组为Base64字符串
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Base64 string</returns>
        public static string ToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
        }

        /// <summary>
        /// Hexadecimal string to bytes
        /// 16进制字符串转化为字节数组
        /// </summary>
        /// <param name="hexString">Hexadecimal string</param>
        /// <returns>Bytes</returns>
        public static byte[] FromHexadecimalString(string hexString)
        {
            var numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Convert bytes to Hexadecimal string
        /// 转化字节数组为16进制字符串
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Hexadecimal string</returns>
        public static string ToHexadecimalString(byte[] bytes)
        {
            return string.Join(string.Empty, bytes.Select(x => x.ToString("x2")));
        }
    }
}
