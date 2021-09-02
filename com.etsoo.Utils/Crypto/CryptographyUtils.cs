using com.etsoo.Utils.SpanMemory;
using Microsoft.IO;
using Microsoft.Toolkit.HighPerformance;
using System.Security.Cryptography;
using System.Text;

namespace com.etsoo.Utils.Crypto
{
    /// <summary>
    /// Cryptography Tools
    /// 密码工具
    /// </summary>
    public static class CryptographyUtils
    {
        private static (byte[], byte[]) AESManagedCreate(string passPhrase)
        {
            // Random byte
            // 随机字节
            var randByte = (byte)(passPhrase.Length % 128);

            // Hash the user password along with the salt
            // 由密匙创建加密的键，而不要直接使用密匙
            // https://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            using var password = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(passPhrase), new byte[] { 0, 1, 2, randByte, 4, 5, 6, 7 }, 10000);

            // Key: 32 x 8 = 256 bits
            // IV: 16 x 8 = 128 bits
            return (password.GetBytes(32), password.GetBytes(16));
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
        public static byte[] AESDecrypt(byte[] cipherTextBytes, string passPhrase)
        {
            using var aes = Aes.Create();
            var (key, iv) = AESManagedCreate(passPhrase);
            aes.Key = key;
            return aes.DecryptCbc(cipherTextBytes, iv);
        }

        /// <summary>
        /// AES (CBC) 256-bit symmetric encryption
        /// AES 256位对称加密
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passPhrase">Password phrase</param>
        /// <returns>Encrypted bytes</returns>
        public static byte[] AESEncrypt(string plainText, string passPhrase)
        {
            using var aes = Aes.Create();
            var (key, iv) = AESManagedCreate(passPhrase);
            aes.Key = key;
            return aes.EncryptCbc(Encoding.UTF8.GetBytes(plainText), iv);
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

            // Generator
            RandomNumberGenerator.Create().GetBytes(bytes);

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
        /// Hash-based Message Authentication Code (HMAC), SHA256
        /// 基于哈希的消息认证码（HMAC）, SHA256
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed bytes</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static byte[] HMACSHA256(ReadOnlySpan<char> message, ReadOnlySpan<char> privateKey)
        {
            // HMAC
            using var alg = new HMACSHA256(privateKey.ToEncodingBytes().ToArray());

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
        public static byte[] HMACSHA512(ReadOnlySpan<char> message, ReadOnlySpan<char> privateKey)
        {
            // HMAC
            using var alg = new HMACSHA512(privateKey.ToEncodingBytes().ToArray());

            alg.Initialize();

            var manager = new RecyclableMemoryStreamManager();

            return alg.ComputeHash(manager.GetStream(message.ToEncodingBytes().ToArray()));
        }

        /// <summary>
        /// Hash-based Message Authentication Code (HMAC), SHA256
        /// 基于哈希的消息认证码（HMAC）, SHA256
        /// </summary>
        /// <param name="message">Raw message</param>
        /// <param name="privateKey">Private key</param>
        /// <returns>Hashed bytes</returns>
        /// <seealso href="http://www.baike.com/wiki/HMAC/">HMAC</seealso>
        public static async Task<byte[]> HMACSHA256Async(string message, string privateKey)
        {
            // HMAC
            using var alg = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey));

            alg.Initialize();

            var manager = new RecyclableMemoryStreamManager();

            return await alg.ComputeHashAsync(manager.GetStream(Encoding.UTF8.GetBytes(message)));
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

            var manager = new RecyclableMemoryStreamManager();

            return await alg.ComputeHashAsync(manager.GetStream(Encoding.UTF8.GetBytes(message)));
        }
    }
}
