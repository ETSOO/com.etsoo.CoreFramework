using com.etsoo.Utils.SpanMemory;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace com.etsoo.Utils.Crypto
{
    /// <summary>
    /// RAS crypto, PKCS1
    /// RSA can encrypt data to a maximum amount of your key size (2048 bits = 256 bytes) 
    /// RAS加密
    /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa?view=net-6.0
    /// </summary>
    public class RSACrypto
    {
        [RequiresDynamicCode("IConfigurationSection may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("IConfigurationSection may require dynamic access otherwise can break functionality when trimming application code")]
        private static (string? publicKey, string? privateKey) Parse(IConfigurationSection section, Func<string, string, string>? secureManager)
        {
            return Parse(section.GetValue<string?>("PublicKey"), section.GetValue<string?>("PrivateKey"), secureManager);
        }

        private static (string? publicKey, string? privateKey) Parse(string? publicKey, string? privateKey, Func<string, string, string>? secureManager)
        {
            return (
                CryptographyUtils.UnsealData(nameof(publicKey), publicKey, secureManager),
                CryptographyUtils.UnsealData(nameof(privateKey), privateKey, secureManager)
            );
        }

        /// <summary>
        /// RSA instance
        /// RAS实例
        /// </summary>
        public RSA RSA { get; private set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="keys">RAS public & private Keys</param>
        public RSACrypto((string? publicKey, string? privateKey) keys)
        {
            var (publicKey, privateKey) = keys;

            var hasPublicKey = !string.IsNullOrEmpty(publicKey);
            var hasPrivateKey = !string.IsNullOrEmpty(privateKey);
            if (!hasPublicKey && !hasPrivateKey)
            {
                // ArgumentNullException.ThrowIfNull(publicKey);
                throw new ArgumentNullException(nameof(keys));
            }

            RSA = RSA.Create();

            if (hasPrivateKey)
                RSA.ImportRSAPrivateKey(Convert.FromBase64String(privateKey!), out _);
            else if (hasPublicKey)
                RSA.ImportRSAPublicKey(Convert.FromBase64String(publicKey!), out _);
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        /// <param name="secureManager">Secure manager</param>
        [RequiresDynamicCode("section may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("section may require dynamic access otherwise can break functionality when trimming application code")]
        public RSACrypto(IConfigurationSection section, Func<string, string, string>? secureManager = null)
            : this(Parse(section, secureManager))
        {
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="publicKey">Public key</param>
        /// <param name="privateKey">Public key</param>
        /// <param name="secureManager">Secure manager</param>
        public RSACrypto(string? publicKey, string? privateKey, Func<string, string, string>? secureManager = null)
            : this(Parse(publicKey, privateKey, secureManager))
        {
        }

        /// <summary>
        /// Decrypt
        /// 解密
        /// </summary>
        /// <param name="data">Base64 Data</param>
        /// <param name="padding">Encryption padding</param>
        /// <returns>Result</returns>
        public byte[] Decrypt(ReadOnlySpan<char> data, RSAEncryptionPadding? padding = null)
        {
            return Decrypt(data.ToBase64Bytes().ToArray(), padding);
        }

        /// <summary>
        /// Decrypt
        /// 解密
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="padding">Encryption padding</param>
        /// <returns>Result</returns>
        public byte[] Decrypt(byte[] data, RSAEncryptionPadding? padding = null)
        {
            return RSA.Decrypt(data, padding ?? RSAEncryptionPadding.OaepSHA512);
        }

        /// <summary>
        /// Encrypt, keep an eye on plainText size
        /// https://stackoverflow.com/questions/64491574/in-explicable-error-with-rsa-encrypt-with-rsaencryptionpadding-oaepsha256-base
        /// 加密
        /// </summary>
        /// <param name="data">Raw UTF8 data</param>
        /// <param name="padding">Encryption padding</param>
        /// <returns>Result</returns>
        public byte[] Encrypt(ReadOnlySpan<char> data, RSAEncryptionPadding? padding = null)
        {
            return Encrypt(data.ToEncodingBytes().ToArray(), padding);
        }

        /// <summary>
        /// Encrypt, keep an eye on plainText size
        /// https://stackoverflow.com/questions/64491574/in-explicable-error-with-rsa-encrypt-with-rsaencryptionpadding-oaepsha256-base
        /// 加密
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="padding">Encryption padding</param>
        /// <returns>Result</returns>
        public byte[] Encrypt(byte[] data, RSAEncryptionPadding? padding = null)
        {
            return RSA.Encrypt(data, padding ?? RSAEncryptionPadding.OaepSHA512);
        }

        /// <summary>
        /// Sign data, with private key only
        /// 数据签名
        /// </summary>
        /// <param name="data">Raw UTF8 data</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="padding">Signature padding</param>
        /// <returns>RSA signature</returns>
        public byte[] SignData(ReadOnlySpan<char> data, HashAlgorithmName? hashAlgorithm = null, RSASignaturePadding? padding = null)
        {
            return SignData(data.ToEncodingBytes().ToArray(), hashAlgorithm, padding);
        }

        /// <summary>
        /// Sign data, with private key only
        /// 数据签名
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="padding">Signature padding</param>
        /// <returns>RSA signature</returns>
        public byte[] SignData(byte[] data, HashAlgorithmName? hashAlgorithm = null, RSASignaturePadding? padding = null)
        {
            return RSA.SignData(data, hashAlgorithm ?? HashAlgorithmName.SHA512, padding ?? RSASignaturePadding.Pss);
        }

        /// <summary>
        /// Verify data
        /// 验证签名数据
        /// </summary>
        /// <param name="data">Base64 data</param>
        /// <param name="signature">Base64 signature</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="padding">Signature padding</param>
        /// <returns>Result</returns>
        public bool VerifyData(ReadOnlySpan<char> data, ReadOnlySpan<char> signature, HashAlgorithmName? hashAlgorithm = null, RSASignaturePadding? padding = null)
        {
            return VerifyData(data.ToBase64Bytes().ToArray(), signature.ToEncodingBytes().ToArray(), hashAlgorithm, padding);
        }

        /// <summary>
        /// Verify data
        /// 验证签名数据
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="signature">Signature bytes</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <param name="padding">Signature padding</param>
        /// <returns>Result</returns>
        public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName? hashAlgorithm = null, RSASignaturePadding? padding = null)
        {
            return RSA.VerifyData(data, signature, hashAlgorithm ?? HashAlgorithmName.SHA512, padding ?? RSASignaturePadding.Pss);
        }
    }
}