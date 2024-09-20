using System.Net;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User token interface
    /// 用户令牌接口
    /// </summary>
    [JsonDerivedType(typeof(ICurrentUser))]
    [JsonDerivedType(typeof(UserToken))]
    [JsonDerivedType(typeof(CurrentUser))]
    public interface IUserToken : IMinUserToken
    {
        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        string Organization { get; }

        /// <summary>
        /// Int organization id
        /// 整数机构编号
        /// </summary>
        int OrganizationInt { get; }

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

        /// <summary>
        /// Int device id
        /// 整数设备编号
        /// </summary>
        int DeviceIdInt { get; }
    }
}
