using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Database;
using com.etsoo.Utils;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
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
        /// Init call encryption identifier
        /// </summary>
        protected const string InitCallEncryptionIdentifier = "InitCall";

        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        protected virtual ICoreApplication<C> App { get; }

        /// <summary>
        /// Database repository
        /// 数据库仓库
        /// </summary>
        protected virtual R Repo { get; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Cancellation token
        /// 取消令牌
        /// </summary>
        protected readonly CancellationToken CancellationToken;

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
        /// <param name="cancellationToken">Cancellation token</param>
        public ServiceBase(ICoreApplication<C> app, R repo, ILogger logger, CancellationToken cancellationToken = default)
        {
            App = app;
            Repo = repo;
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Decrypt message
        /// 解密信息
        /// </summary>
        /// <param name="encyptedMessage">Encrypted message</param>
        /// <param name="passphrase">Secret passphrase</param>
        /// <param name="durationSeconds">Duration seconds, <= 12 will be considered as month</param>
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
                    var ts = DateTime.UtcNow - SharedUtils.JsMilisecondsToUTC(miliseconds);

                    // Month
                    if (durationSeconds.Value <= 12 && (ts.TotalDays > 30 * durationSeconds.Value || ts.TotalDays <= 0)) return null;

                    // Seconds
                    if (durationSeconds.Value > 12 && Math.Abs(ts.TotalSeconds) > durationSeconds.Value) return null;
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
        /// Decrypt device core with user identifier
        /// 使用用户识别码解密设备核心
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="identifier">User identifier</param>
        /// <returns>Result</returns>
        public string? DecryptDeviceCore(string deviceId, string identifier)
        {
            // Valid within 30 days / one month
            return Decrypt(deviceId, identifier, 1);
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
                var miliseconds = SharedUtils.UTCToJsMiliseconds();
                var timeStamp = StringUtils.NumberToChars(miliseconds);
                return timeStamp + "!" + CryptographyUtils.AESEncrypt(message, EncryptionEnhance(passphrase, timeStamp), iterations);
            }

            return CryptographyUtils.AESEncrypt(message, passphrase, iterations);
        }

        /// <summary>
        /// Encrypt message to be decrpted on web side with app.decrypt(messageEncrypted), passphrase passed with web's deviceId
        /// 加密用于Web端解密的信息
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="passphrase">Secret passphrase</param>
        /// <returns>Result</returns>
        protected string EncryptWeb(string message, string passphrase)
        {
            return Encrypt(message, passphrase, 1, false);
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
        /// Decrypt data with application hash
        /// 使用应用程序哈希解密数据
        /// </summary>
        /// <param name="encryptedData">Encypted data</param>
        /// <param name="identifier">Hash identifier</param>
        /// <returns>Result</returns>
        protected virtual async Task<string?> HashDecryptAsync(string encryptedData, string identifier)
        {
            var phassphrase = await App.HashPasswordAsync(identifier + identifier.Length);
            return Decrypt(encryptedData, phassphrase);
        }

        /// <summary>
        /// Encrypt data with application hash
        /// 使用应用程序哈希加密数据
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="identifier">Hash identifier</param>
        /// <returns>Result</returns>
        protected virtual async Task<string> HashEncryptAsync(string data, string identifier)
        {
            var phassphrase = await App.HashPasswordAsync(identifier + identifier.Length);
            return Encrypt(data, phassphrase, 10, false);
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
            var clientDT = SharedUtils.JsMilisecondsToUTC(rq.Timestamp);
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

            var result = ActionResult.Success;
            var clientPassphrase = rq.Timestamp.ToString();

            // Random bytes
            var randomChars = Convert.ToBase64String(CryptographyUtils.CreateRandBytes(32));

            // New device id
            var newDeviceId = Encrypt(randomChars, secret);

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

                    // Repo update
                    if (!string.IsNullOrEmpty(rq.Identifier))
                    {
                        var source = await HashDecryptAsync(rq.Identifier, InitCallEncryptionIdentifier);
                        if (source != null && int.TryParse(source, out var deviceId))
                        {
                            await InitCallUpdateAsync(rq.DeviceId, newDeviceId, deviceId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return LogException(ex);
                }
            }

            // Return to the client side
            result.Data.Add("DeviceId", newDeviceId);
            result.Data.Add("Passphrase", CryptographyUtils.AESEncrypt(randomChars, clientPassphrase, 1));

            return result;
        }

        /// <summary>
        /// Async init call update
        /// 异步初始化调用更新
        /// </summary>
        /// <param name="prevDeviceId">Previous client device id</param>
        /// <param name="newDeviceId">New client device id</param>
        /// <param name="deviceId">Serverside device id</param>
        /// <returns>Task</returns>
        protected virtual async Task InitCallUpdateAsync(string prevDeviceId, string newDeviceId, int deviceId)
        {
            await Task.CompletedTask;
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
#pragma warning disable CA2254
            if (critical)
                Logger.LogCritical(ex, message);
            else
                Logger.LogError(ex, message);
#pragma warning restore CA2254
        }
    }
}
