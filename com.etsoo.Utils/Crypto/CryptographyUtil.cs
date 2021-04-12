using com.etsoo.Utils.SpanMemory;
using Microsoft.IO;
using Microsoft.Toolkit.HighPerformance;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Crypto
{
    /// <summary>
    /// Cryptography Tools
    /// 密码工具
    /// </summary>
    public static class CryptographyUtil
    {
        private static AesManaged AESManagedCreate(ReadOnlySpan<char> passPhrase)
        {
            // Random byte
            // 随机字节
            var randByte = (byte)(passPhrase.Length % 128);

            // Hash the user password along with the salt
            // 由密匙创建加密的键，而不要直接使用密匙
            // https://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            using var password = new Rfc2898DeriveBytes(passPhrase.ToEncodingBytes().ToArray(), new byte[] { 0, 1, 2, randByte, 4, 5, 6, 7 }, 10000);

            // 32 x 8 = 256 bits
            // 16 x 8 = 128 bits
            return new AesManaged()
            {
                Key = password.GetBytes(32),
                IV = password.GetBytes(16),
                Mode = CipherMode.CBC // Default
            };
        }

        /// <summary>
        /// AES (CBC) 256-bit symmetric decryption
        /// https://thehftguy.com/2020/04/20/what-aes-ciphers-to-use-between-cbc-gcm-ccm-chacha-poly/
        /// https://stackoverflow.com/questions/1220751/how-to-choose-an-aes-encryption-mode-cbc-ecb-ctr-ocb-cfb
        /// AES 256位对称解密
        /// </summary>
        /// <param name="cipherTextBytes">Cipher text bytes</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<ReadOnlyMemory<char>> AESDecryptAsync(ReadOnlyMemory<byte> cipherTextBytes, ReadOnlyMemory<char> passPhrase)
        {
            using var aesAlg = AESManagedCreate(passPhrase.Span);
            using var decryptor = aesAlg.CreateDecryptor();
            using var csDecrypt = new CryptoStream(cipherTextBytes.AsStream(), decryptor, CryptoStreamMode.Read, true);
            using var srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8);
            return await srDecrypt.ReadAllCharsAsyn();
        }

        /// <summary>
        /// AES 256-bit symmetric decryption from base64 string
        /// AES 256位对称解密，从Base64字符串
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<ReadOnlyMemory<char>> AESDecryptFromBase64Async(ReadOnlyMemory<char> cipherText, ReadOnlyMemory<char> passPhrase)
        {
            return await AESDecryptAsync(Convert.FromBase64String(cipherText.ToString()).AsMemory(), passPhrase);
        }

        /// <summary>
        /// AES 256-bit symmetric decryption from hexadecimal string
        /// AES 256位对称解密，从16进制字符串
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<ReadOnlyMemory<char>> AESDecryptFromHexAsync(ReadOnlyMemory<char> cipherText, ReadOnlyMemory<char> passPhrase)
        {
            return await AESDecryptAsync(Convert.FromHexString(cipherText.Span), passPhrase);
        }

        /// <summary>
        /// AES (CBC) 256-bit symmetric encryption
        /// AES 256位对称加密
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static async Task<ReadOnlyMemory<byte>> AESEncryptAsync(ReadOnlyMemory<char> plainText, ReadOnlyMemory<char> passPhrase)
        {
            using var aesAlg = AESManagedCreate(passPhrase.Span);
            using var encryptor = aesAlg.CreateEncryptor();

            var manager = new RecyclableMemoryStreamManager();
            using var msEncrypt = manager.GetStream();

            // leaveOpen = true is critical
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write, true);
            using var swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8);

            // Write the plain text for encryption
            await swEncrypt.WriteAsync(plainText);
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
        public static async Task<ReadOnlyMemory<char>> AESEncryptToBase64Async(ReadOnlyMemory<char> plainText, ReadOnlyMemory<char> passPhrase)
        {
            return Convert.ToBase64String((await AESEncryptAsync(plainText, passPhrase)).ToArray()).AsMemory();
        }

        /// <summary>
        /// AES 256-bit symmetric encryption, to Hexadecimal string
        /// AES 256位对称加密，到16进制字符串
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted hexadecimal string</returns>
        public static async Task<ReadOnlyMemory<char>> AESEncryptToHexAsync(ReadOnlyMemory<char> plainText, ReadOnlyMemory<char> passPhrase)
        {
            return Convert.ToHexString((await AESEncryptAsync(plainText, passPhrase)).Span).AsMemory();
        }

        /// <summary>
        /// Create random bytes
        /// 创建随机字节数组
        /// </summary>
        /// <param name="size">Size</param>
        /// <returns>Random bytes</returns>
        public static ReadOnlySpan<byte> CreateRandBytes(int size)
        {
            // Init
            var bytes = new byte[size].AsSpan();

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
        public static ReadOnlySpan<char> CreateRandString(RandStringKind kind, int size)
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

            // Span builder
            var builder = new SpanBuilder<char>(size);

            foreach (var b in byes)
            {
                builder.Append(collection[b % len]);
            }

            // Return
            return builder.AsSpan();
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512
        /// 基于哈希的消息认证码（HMAC）, SHA512
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed bytes</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static byte[] HMACSHA512(ReadOnlySpan<char> message, ReadOnlySpan<char> privateKey)
        {
            // HMAC
            using var alg = new HMACSHA512(privateKey.ToEncodingBytes().ToArray());

            alg.Initialize();

            var manager = new RecyclableMemoryStreamManager();

            return alg.ComputeHash(manager.GetStream(message.ToEncodingBytes().ToArray()));
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512
        /// 基于哈希的消息认证码（HMAC）, SHA512
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed bytes</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<ReadOnlyMemory<byte>> HMACSHA512Async(ReadOnlyMemory<char> message, ReadOnlyMemory<char> privateKey)
        {
            // HMAC
            using var alg = new HMACSHA512(privateKey.Span.ToEncodingBytes().ToArray());

            alg.Initialize();

            var manager = new RecyclableMemoryStreamManager();

            return await alg.ComputeHashAsync(manager.GetStream(message.ToEncodingBytes()));
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512 to Base64 string
        /// 基于哈希的消息身份验证代码（HMAC）, SHA512到Base64字符串
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed message</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<ReadOnlyMemory<char>> HMACSHA512ToBase64Async(ReadOnlyMemory<char> message, ReadOnlyMemory<char> privateKey)
        {
            return Convert.ToBase64String((await HMACSHA512Async(message, privateKey)).Span).AsMemory();
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA512 to Hexadecimal string
        /// 基于哈希的消息身份验证代码（HMAC）, SHA512到16进制字符串
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed message</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<ReadOnlyMemory<char>> HMACSHA512ToHexAsync(ReadOnlyMemory<char> message, ReadOnlyMemory<char> privateKey)
        {
            return Convert.ToHexString((await HMACSHA512Async(message, privateKey)).Span).AsMemory();
        }
    }
}
