using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Init call request data
    /// 初始化调用请求数据
    /// </summary>
    public record InitCallRQ : IModelValidator
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public string? DeviceId { get; init; }

        /// <summary>
        /// Serverside identifier, database device id encrypted
        /// 服务器端识别码，数据库端加密的设备编号
        /// </summary>
        public string? Identifier { get; init; }

        /// <summary>
        /// Timestamp, JavaScript miliseconds
        /// 时间戳，JavaScript毫秒数
        /// </summary>
        public required long Timestamp { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public IActionResult? Validate()
        {
            if (DeviceId != null && DeviceId.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(DeviceId));
            }

            if (Identifier != null && Identifier.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Identifier));
            }

            return null;
        }
    }
}
