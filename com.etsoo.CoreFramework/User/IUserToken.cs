using System.Net;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User token interface
    /// 用户令牌接口
    /// </summary>
    public interface IUserToken : IMinUserToken
    {
        /// <summary>
        /// Int id
        /// 整数编号
        /// </summary>
        int IdInt { get; }

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        string? Organization { get; }

        /// <summary>
        /// Int organization id
        /// 整数机构编号
        /// </summary>
        int? OrganizationInt { get; }

        /// <summary>
        /// Client IP address
        /// 客户端IP地址
        /// </summary>
        IPAddress ClientIp { get; }

        /// <summary>
        /// Country or region
        /// 国家或地区
        /// </summary>
        string Region { get; }

        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        string DeviceId { get; }
    }
}
