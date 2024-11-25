using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Public user data
    /// 用户公共数据
    /// </summary>
    public record PublicUserData
    {
        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Given name
        /// 名
        /// </summary>
        public string? GivenName { get; init; }

        /// <summary>
        /// Family name
        /// 姓
        /// </summary>
        public string? FamilyName { get; init; }

        /// <summary>
        /// Latin given name
        /// 拉丁名（拼音）
        /// </summary>
        public string? LatinGivenName { get; init; }

        /// <summary>
        /// Latin family name
        /// 拉丁姓（拼音）
        /// </summary>
        public string? LatinFamilyName { get; init; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        public string? Avatar { get; init; }

        /// <summary>
        /// Organization id
        /// 机构编号
        /// </summary>
        public int? Organization { get; init; }

        /// <summary>
        /// Is accessing as channel organization
        /// 是否作为渠道机构访问
        /// </summary>
        public bool IsChannel { get; init; }

        /// <summary>
        /// Is accessing as parent organization
        /// 是否作为上级机构访问
        /// </summary>
        public bool IsParent { get; init; }

        /// <summary>
        /// User role value
        /// 用户角色值
        /// </summary>
        public required short Role { get; init; }

        /// <summary>
        /// Token scheme
        /// 令牌方案
        /// </summary>
        public string TokenScheme { get; init; } = "Bearer";

        /// <summary>
        /// Access token
        /// 访问令牌
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Token expiration seconds
        /// 令牌过期秒数
        /// </summary>
        public int Seconds { get; init; }

        /// <summary>
        /// Serverside device id
        /// 服务器端设备编号
        /// </summary>
        public string? DeviceId { get; init; }

        /// <summary>
        /// Save to action result
        /// 保存到操作结果
        /// </summary>
        /// <param name="result">Action result</param>
        public virtual void SaveTo(IActionResult result)
        {
            result.Data[nameof(Name)] = Name;
            result.Data[nameof(GivenName)] = GivenName;
            result.Data[nameof(FamilyName)] = FamilyName;
            result.Data[nameof(LatinGivenName)] = LatinGivenName;
            result.Data[nameof(LatinFamilyName)] = LatinFamilyName;
            result.Data[nameof(Avatar)] = Avatar;
            result.Data[nameof(Organization)] = Organization;
            result.Data[nameof(IsChannel)] = IsChannel;
            result.Data[nameof(IsParent)] = IsParent;
            result.Data[nameof(Role)] = Role;
            result.Data[nameof(TokenScheme)] = TokenScheme;
            result.Data[nameof(Token)] = Token;
            result.Data[nameof(Seconds)] = Seconds;
            result.Data[nameof(DeviceId)] = DeviceId;
        }
    }
}
