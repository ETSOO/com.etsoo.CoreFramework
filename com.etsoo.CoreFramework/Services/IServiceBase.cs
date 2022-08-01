using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Service base interface
    /// 基础服务接口
    /// </summary>
    public interface IServiceBase
    {
        /// <summary>
        /// Decrypt device core with user identifier for multiple decryption
        /// 使用用户识别码解密设备核心以用于多次解密
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="identifier">User identifier</param>
        /// <returns>Result</returns>
        string? DecryptDeviceCore(string deviceId, string identifier);

        /// <summary>
        /// Async decrypt device data with passphrase
        /// 使用密码异步解密设备数据
        /// </summary>
        /// <param name="encryptedMessage">Encrypted message</param>
        /// <param name="deviceCore">Device core passphrase</param>
        /// <returns>Result</returns>
        string? DecryptDeviceData(string encryptedMessage, string deviceCore);

        /// <summary>
        /// Log exception and return simple user result
        /// 登记异常结果日志，并返回简介的用户结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        ActionResult LogException(Exception ex);
    }
}
