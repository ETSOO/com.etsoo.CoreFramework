using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace com.etsoo.CoreFramework.Utils
{
    /// <summary>
    /// Cryptography Tools
    /// 密码工具
    /// </summary>
    public static class CryptographyUtil
    {
        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512
        /// 基于哈希的消息认证码（HMAC）, SHA512
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed bytes</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static byte[] HMACSHA512(string message, string privateKey)
        {
            // Validate the message
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            // HMAC
            using var alg = new HMACSHA512(Encoding.UTF8.GetBytes(privateKey));

            alg.Initialize();
            alg.ComputeHash(Encoding.UTF8.GetBytes(message));

            // Return
            return alg.Hash;
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512 to Base64 string
        /// 基于哈希的消息身份验证代码（HMAC）, SHA512到Base64字符串
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed message</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static string HMACSHA512ToBase64(string message, string privateKey)
        {
            return Convert.ToBase64String(HMACSHA512(message, privateKey), Base64FormattingOptions.None);
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512 to Hexadecimal string
        /// 基于哈希的消息身份验证代码（HMAC）, SHA512到16进制字符串
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed message</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static string HMACSHA512ToHex(string message, string privateKey)
        {
            return string.Join(string.Empty, HMACSHA512(message, privateKey).Select(x => x.ToString("x2")));
        }
    }
}
