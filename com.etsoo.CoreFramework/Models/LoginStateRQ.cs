using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Login state request data
    /// 登录状态请求数据
    /// </summary>
    public record LoginStateRQ : SignModel
    {
        /// <summary>
        /// Region
        /// 地区
        /// </summary>
        public required string Region { get; init; }

        /// <summary>
        /// Device
        /// 设备
        /// </summary>
        public required string Device { get; init; }

        /// <summary>
        /// Sign the request with the app secret
        /// 对请求使用应用密钥签名
        /// </summary>
        /// <param name="appSecret">Application secret</param>
        /// <returns>Result</returns>
        public string SignWith(string appSecret)
        {
            var rq = new SortedDictionary<string, string>
            {
                [nameof(Device)] = Device,
                [nameof(Region)] = Region
            };

            return SignWith(rq, appSecret);
        }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public override IActionResult? Validate()
        {
            var result = base.Validate();
            if (result != null)
            {
                return result;
            }

            if (Device.Length is not (>= 2 and <= 256))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Device));
            }

            if (Region.Length is not 2)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Region));
            }

            return null;
        }
    }
}
