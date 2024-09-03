using com.etsoo.UserAgentParser;

namespace com.etsoo.Web
{
    /// <summary>
    /// Authorization login validate data
    /// 授权登录验证数据
    /// </summary>
    public record AuthLoginValidateData
    {
        /// <summary>
        /// Device id from client
        /// 从客户端的设备编号
        /// </summary>
        public required string DeviceId { get; init; }

        /// <summary>
        /// Region
        /// 区域
        /// </summary>
        public required string Region { get; init; }

        /// <summary>
        /// User agent parser
        /// 用户代理解析器
        /// </summary>
        public required UAParser Parser { get; init; }
    }
}
