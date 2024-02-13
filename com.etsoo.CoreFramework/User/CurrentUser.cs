using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data
    /// 当前用户数据
    /// </summary>
    public record CurrentUser : ServiceUser, ICurrentUser, IUserCreator<CurrentUser>
    {
        /// <summary>
        /// Avatar claim type
        /// 头像声明类型
        /// </summary>
        public const string AvatarClaim = "avatar";

        /// <summary>
        /// Is Corporate claim type
        /// 是否为法人申明类型
        /// </summary>
        public const string CorporateClaim = "Corporate";

        /// <summary>
        /// Organization name claim type
        /// 机构名称申明类型
        /// </summary>
        public const string OrganizationNameClaim = "OrgName";

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static new CurrentUser? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            if (claims == null) return null;

            var user = ServiceUser.Create(claims);
            if (user == null) return null;

            // Claims
            var name = claims.FindFirstValue(ClaimTypes.Name);
            var orgName = claims.FindFirstValue(OrganizationNameClaim);
            var avatar = claims.FindFirstValue(AvatarClaim);

            // Is corporate
            var corporate = StringUtils.TryParse<bool>(claims.FindFirstValue(CorporateClaim)).GetValueOrDefault();

            // Validate
            if (string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                user.Id,
                user.Uid,
                user.Organization,
                name, user.RoleValue,
                user.ClientIp,
                user.DeviceId,
                user.Language,
                user.Region,
                corporate,
                connectionId)
            {
                Service = user.Service,
                Avatar = avatar,
                OrganizationName = orgName
            };
        }

        private static (string? Name, string? OrgName, string? Avatar, string? JsonData, bool Corporate) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Name"),
                data.Get("OrgName"),
                data.Get("Avatar"),
                data.Get("JsonData"),
                data.Get("Corporate", false)
            );
        }

        /// <summary>
        /// Create user from result data
        /// 从操作结果数据创建用户
        /// </summary>
        /// <param name="data">Result data</param>
        /// <param name="ip">Ip address</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static new CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region, string? connectionId = null)
        {
            // Base
            var token = ServiceUser.Create(data, ip, language, region);
            if (token == null) return null;

            // Get data
            var (name, orgName, avatar, jsonData, corporate) = GetData(data);

            // Validation
            if (string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                token.Id,
                token.Uid,
                token.Organization,
                name,
                token.RoleValue,
                ip,
                token.DeviceId,
                language,
                region,
                corporate,
                connectionId)
            {
                Avatar = avatar,
                OrganizationName = orgName,
                JsonData = jsonData
            };
        }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        public string? OrganizationName { get; set; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        public string? Avatar { get; private set; }

        /// <summary>
        /// Json data
        /// Json数据
        /// </summary>
        public string? JsonData { get; set; }

        /// <summary>
        /// Is Corporate
        /// 是否为法人
        /// </summary>
        public bool Corporate { get; private set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="uid">Uid</param>
        /// <param name="organization">Organization</param>
        /// <param name="name">Name</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="corporate">Is corporate</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, string? uid, string? organization, string name, short roleValue, IPAddress clientIp, int deviceId, CultureInfo language, string region, bool? corporate = null, string? connectionId = null)
            : base(id, uid, organization, roleValue, clientIp, deviceId, language, region, connectionId)
        {
            Name = name;
            Corporate = corporate.GetValueOrDefault();
        }

        private IEnumerable<Claim> GetClaims()
        {
            yield return new(ClaimTypes.Name, Name);
            yield return new(CorporateClaim, Corporate.ToJson());

            if (!string.IsNullOrEmpty(OrganizationName))
                yield return new(OrganizationNameClaim, OrganizationName);

            if (Avatar != null)
                yield return new(AvatarClaim, Avatar);
            if (!string.IsNullOrEmpty(JsonData))
                yield return new(ClaimTypes.UserData, JsonData);
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public override IEnumerable<Claim> CreateClaims()
        {
            var claims = GetClaims();
            return base.CreateClaims().Concat(claims);
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        public override void Update(StringKeyDictionaryObject data)
        {
            base.Update(data);

            // Editable fields
            var (name, orgName, avatar, jsonData, corporate) = GetData(data);

            // Name
            if (name != null)
                Name = name;

            OrganizationName = orgName;

            // Corporate
            Corporate = corporate;

            // Avatar
            Avatar = avatar;

            // Json data
            JsonData = jsonData;
        }
    }
}
