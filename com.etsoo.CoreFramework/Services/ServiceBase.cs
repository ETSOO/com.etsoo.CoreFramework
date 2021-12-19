using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.Localization;
using com.etsoo.Utils.String;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Text;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Service base for business logic
    /// 业务逻辑的基础服务
    /// </summary>
    public abstract class ServiceBase<C, R> : IServiceBase
        where C : DbConnection
        where R : IRepoBase
    {
        // Duration seconds for time span of the server side and browser/client side
        private const int DurationSeconds = 120;

        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        virtual protected ICoreApplication<C> App { get; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Database repository
        /// 数据库仓库
        /// </summary>
        protected R Repo { get; }

        /// <summary>
        /// Secret passphrase
        /// 安全密码
        /// </summary>
        protected string? Passphrase { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public ServiceBase(ICoreApplication<C> app, R repo, ILogger logger)
        {
            App = app;
            Repo = repo;
            Logger = logger;
        }

        /// <summary>
        /// Async create hashed passphrase
        /// 异步创建哈希密码
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        protected virtual async Task<string> CreateHashedPassphraseAsync(string input)
        {
            var hashBytes = await App.HashPasswordBytesAsync(input);
            return Convert.ToHexString(hashBytes);
        }

        /// <summary>
        /// Decrypt message
        /// 解密信息
        /// </summary>
        /// <param name="encyptedMessage">Encrypted message</param>
        /// <param name="passphrase">Secret passphrase</param>
        /// <param name="durationSeconds">Duration seconds</param>
        /// <param name="isWebClient">Is web client</param>
        /// <returns>Result</returns>
        protected virtual string? Decrypt(string encyptedMessage, string passphrase, int? durationSeconds = null, bool? isWebClient = null)
        {
            var pos = encyptedMessage.IndexOf('!');
            if (pos >= 8)
            {
                // Miliseconds chars are longer than 8
                var timestamp = encyptedMessage[..pos];

                if (durationSeconds.HasValue)
                {
                    var miliseconds = StringUtils.CharsToNumber(timestamp);
                    var ts = DateTime.UtcNow - LocalizationUtils.JsMilisecondsToUTC(miliseconds);
                    if (Math.Abs(ts.TotalSeconds) > durationSeconds.Value) return null;
                }

                if (isWebClient.GetValueOrDefault()) passphrase = WebEncryptionEnhance(passphrase, timestamp);
                else passphrase = EncryptionEnhance(passphrase, timestamp);

                encyptedMessage = encyptedMessage[(pos + 1)..];
            }

            var bytes = CryptographyUtils.AESDecrypt(encyptedMessage, passphrase);
            if (bytes == null) return null;

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Async decrypt device data with user identifier for multiple decryption
        /// 使用用户识别码数据异步解密设备数据以用于多次解密
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="encryptedMessage">Encrypted message</param>
        /// <param name="identifier">User identifier</param>
        /// <returns>Result</returns>
        public async Task<string?> DecryptDeviceDataAsync(string deviceId, string encryptedMessage, string identifier)
        {
            // Device core
            var core = await DecryptDeviceCoreAsync(deviceId, identifier);
            if (core == null) return null;
            return DecryptDeviceData(encryptedMessage, core);
        }

        /// <summary>
        /// Async decrypt device core with user identifier
        /// 使用用户识别码异步解密设备核心
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="identifier">User identifier</param>
        /// <returns>Result</returns>
        public async Task<string?> DecryptDeviceCoreAsync(string deviceId, string identifier)
        {
            var passphrase = await CreateHashedPassphraseAsync(identifier);
            return Decrypt(deviceId, passphrase);
        }

        /// <summary>
        /// Async decrypt device data with passphrase
        /// 使用密码异步解密设备数据
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="encryptedMessage">Encrypted message</param>
        /// <param name="passphrase">Passphrase</param>
        /// <returns>Result</returns>
        public string? DecryptDeviceData(string deviceId, string encryptedMessage, string passphrase)
        {
            // Device core
            var core = Decrypt(deviceId, passphrase);
            if (core == null) return null;
            return DecryptDeviceData(encryptedMessage, core);
        }

        /// <summary>
        /// Async decrypt device data with passphrase
        /// 使用密码异步解密设备数据
        /// </summary>
        /// <param name="encryptedMessage">Encrypted message</param>
        /// <param name="deviceCore">Device core passphrase</param>
        /// <returns>Result</returns>
        public string? DecryptDeviceData(string encryptedMessage, string deviceCore)
        {
            return Decrypt(encryptedMessage, deviceCore);
        }

        /// <summary>
        /// Enhanced encrypt message
        /// 加强的加密信息
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="passphrase">Secret passphrase</param>
        /// <param name="iterations">Iterations</param>
        /// <param name="enhanced">Enhanced or not</param>
        /// <returns>Result</returns>
        protected virtual string Encrypt(string message, string passphrase, int iterations = 10, bool? enhanced = null)
        {
            if (enhanced.GetValueOrDefault(true))
            {
                var miliseconds = LocalizationUtils.UTCToJsMiliseconds();
                var timeStamp = StringUtils.NumberToChars(miliseconds);
                return timeStamp + "!" + CryptographyUtils.AESEncrypt(message, EncryptionEnhance(passphrase, timeStamp), iterations);
            }

            return CryptographyUtils.AESEncrypt(message, passphrase, iterations);
        }

        /// <summary>
        /// Enchance secret passphrase
        /// 加强安全密码
        /// </summary>
        /// <param name="passphrase">Passphrase</param>
        /// <param name="timeStamp">timeStamp</param>
        /// <returns>Result</returns>
        protected virtual string EncryptionEnhance(string passphrase, string timeStamp)
        {
            return App.HashPassword("Client" + WebEncryptionEnhance(passphrase, timeStamp));
        }

        /// <summary>
        /// Web client enchance secret passphrase
        /// Web客户端加强安全密码
        /// </summary>
        /// <param name="passphrase">Passphrase</param>
        /// <param name="timeStamp">timeStamp</param>
        /// <returns>Result</returns>
        protected virtual string WebEncryptionEnhance(string passphrase, string timeStamp)
        {
            passphrase += timeStamp;
            passphrase += passphrase.Length.ToString();
            return passphrase;
        }

        /// <summary>
        /// Async init call
        /// 异步初始化调用
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <param name="secret">Encryption secret</param>
        /// <returns>Result</returns>
        public async ValueTask<ActionResult> InitCallAsync(InitCallRQ rq, string secret)
        {
            // Check timestamp
            var clientDT = LocalizationUtils.JsMilisecondsToUTC(rq.Timestamp);
            var ts = DateTime.UtcNow - clientDT;
            var validSeconds = DurationSeconds / 2;
            var seconds = Math.Abs(ts.TotalSeconds);
            if (seconds > validSeconds)
            {
                var failure = new ActionResult { Title = "timeDifferenceInvalid" };
                failure.Data.Add("Seconds", seconds);
                failure.Data.Add("ValidSeconds", validSeconds);
                return failure;
            }

            return await Task.Run(() =>
            {
                var result = ActionResult.Success;
                var clientPassphrase = rq.Timestamp.ToString();

                // Check device id for encrypted passphrase
                // secret for decryption
                // Timestamp for client side decryption
                if (!string.IsNullOrEmpty(rq.DeviceId))
                {
                    try
                    {
                        var previousPassphrase = Decrypt(rq.DeviceId, secret);
                        if (previousPassphrase == null) return ApplicationErrors.NoValidData.AsResult("DeviceId");
                        result.Data.Add("PreviousPassphrase", CryptographyUtils.AESEncrypt(previousPassphrase, clientPassphrase, 1));
                    }
                    catch (Exception ex)
                    {
                        return LogException(ex);
                    }
                }

                // Random bytes
                var randomChars = Convert.ToBase64String(CryptographyUtils.CreateRandBytes(32));

                // New device id
                var newDeviceId = Encrypt(randomChars, secret);

                // Return to the client side
                result.Data.Add("DeviceId", newDeviceId);
                result.Data.Add("Passphrase", CryptographyUtils.AESEncrypt(randomChars, clientPassphrase, 1));

                return result;
            });
        }

        /// <summary>
        /// Log exception and return simple user result
        /// 登记异常结果日志，并返回简介的用户结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public ActionResult LogException(Exception ex)
        {
            // Get the Db connection failure result
            var exResult = App.DB.GetExceptionResult(ex);

            // Transform
            var result = exResult.Type switch
            {
                DbExceptionType.OutOfMemory => ApplicationErrors.OutOfMemory.AsResult(),
                DbExceptionType.ConnectionFailed => ApplicationErrors.DbConnectionFailed.AsResult(),
                _ => ApplicationErrors.DataProcessingFailed.AsResult()
            };

            // Log the exception
            LogException(ex, result.Title!, exResult.Critical);

            // Return
            return result;
        }

        /// <summary>
        /// Log exception
        /// 登记异常日志
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message">Message</param>
        /// <param name="critical">Is critical</param>
        protected void LogException(Exception ex, string message, bool critical = false)
        {
            if (critical)
                Logger.LogCritical(ex, message);
            else
                Logger.LogError(ex, message);
        }
    }
}
