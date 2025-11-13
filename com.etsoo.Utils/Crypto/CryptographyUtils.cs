using com.etsoo.Utils.SpanMemory;
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
        /// <summary>
        /// AES (CBC) 256-bit symmetric decryption
        /// AES (CBC) 256位对称解密
        /// https://thehftguy.com/2020/04/20/what-aes-ciphers-to-use-between-cbc-gcm-ccm-chacha-poly/
        /// https://stackoverflow.com/questions/1220751/how-to-choose-an-aes-encryption-mode-cbc-ecb-ctr-ocb-cfb
        /// </summary>
        /// <param name="cipherText">Iterations + salt(Hex) + iv(Hex) + cipher(Base64)</param>
        /// <param name="passphrase">Passphrase</param>
        /// <returns>Result</returns>
        public static byte[]? AESDecrypt(string cipherText, string passphrase)
        {
            if (!int.TryParse(cipherText[..2], out var iterations) || cipherText.Length <= 66)
            {
                return null;
            }

            var salt = cipherText[2..34];
            var iv = cipherText[34..66];
            var encrypted = cipherText[66..];
            var key = PBKDF2(Encoding.UTF8.GetBytes(passphrase), Convert.FromHexString(salt), 32, iterations * 1000);

            using var aes = Aes.Create();
            aes.Key = key;
            return aes.DecryptCbc(Convert.FromBase64String(encrypted), Convert.FromHexString(iv), PaddingMode.PKCS7);
        }

        /// <summary>
        /// PBKDF2 key derivation
        /// PBKDF2 密钥派生
        /// </summary>
        /// <param name="passphrase"></param>
        /// <param name="salt"></param>
        /// <param name="bytes"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static byte[] PBKDF2(byte[] passphrase, byte[] salt, int bytes = 32, int iterations = 10000)
        {
            // Hash the user password along with the salt
            // 由密匙创建加密的键，而不要直接使用密匙
            // https://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            return Rfc2898DeriveBytes.Pbkdf2(passphrase, salt, iterations, HashAlgorithmName.SHA256, bytes);
        }

        /// <summary>
        /// AES (CBC) 256-bit symmetric encryption
        /// AES 256位对称加密
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="passphrase">Passphrase</param>
        /// <param name="iterations">Iterations for key calculation</param>
        /// <returns>Encrypted string, Iterations + salt(Hex) + iv(Hex) + cipher(Base64)</returns>
        public static string AESEncrypt(string plainText, string passphrase, int iterations = 10)
        {
            var salt = CreateRandBytes(16).ToArray();
            var iv = CreateRandBytes(16);
            var key = PBKDF2(Encoding.UTF8.GetBytes(passphrase), salt, 32, iterations * 1000);

            using var aes = Aes.Create();
            aes.Key = key;

            var sb = new StringBuilder();
            sb.Append(iterations.ToString().PadLeft(2, '0'));
            sb.Append(Convert.ToHexString(salt));
            sb.Append(Convert.ToHexString(iv));
            sb.Append(Convert.ToBase64String(aes.EncryptCbc(Encoding.UTF8.GetBytes(plainText), iv)));
            return sb.ToString();
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

            using var ms = SharedUtils.GetStream(message.ToEncodingBytes());

            return alg.ComputeHash(ms);
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

            using var ms = SharedUtils.GetStream(message.ToEncodingBytes());

            return alg.ComputeHash(ms);
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

            await using var ms = SharedUtils.GetStream(message);

            return await alg.ComputeHashAsync(ms);
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

            await using var ms = SharedUtils.GetStream(message);

            return await alg.ComputeHashAsync(ms);
        }

        /// <summary>
        /// MD5 hash
        /// MD5哈希
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Bytes</returns>
        public static async Task<byte[]> MD5Async(string source)
        {
            return await MD5Async(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// MD5 hash
        /// MD5哈希
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Bytes</returns>
        public static async Task<byte[]> MD5Async(byte[] bytes)
        {
            using var md5 = MD5.Create();
            await using var ms = SharedUtils.GetStream(bytes);
            return await md5.ComputeHashAsync(ms);
        }

        /// <summary>
        /// SHA1 hash
        /// SHA1哈希
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Bytes</returns>
        public static async Task<byte[]> SHA1Async(string source)
        {
            return await SHA1Async(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// SHA1 hash
        /// SHA1哈希
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Bytes</returns>
        public static async Task<byte[]> SHA1Async(byte[] bytes)
        {
            using var sha1 = SHA1.Create();
            await using var ms = SharedUtils.GetStream(bytes);
            return await sha1.ComputeHashAsync(ms);
        }

        /// <summary>
        /// SHA3(512) hash
        /// SHA3哈希
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Bytes</returns>
        public static async Task<byte[]> SHA3Async(string source)
        {
            return await SHA3Async(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// SHA3(512) hash
        /// SHA3哈希
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Bytes</returns>
        public static async Task<byte[]> SHA3Async(byte[] bytes)
        {
            using var sha512 = SHA512.Create();
            await using var ms = SharedUtils.GetStream(bytes);
            return await sha512.ComputeHashAsync(ms);
        }
    }
}
