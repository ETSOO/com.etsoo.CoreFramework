﻿using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data
    /// 当前用户数据
    /// </summary>
    public record CurrentUser : ServiceUser, IServiceUser
    {
        /// <summary>
        /// Avatar claim type
        /// 头像声明类型
        /// </summary>
        public const string AvatarClaim = "avatar";

        /// <summary>
        /// Is Corporate
        /// 是否为法人
        /// </summary>
        public const string CorporateClaim = "Corporate";

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static CurrentUser? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            var user = ServiceUser.Create(claims);
            if (user == null) return null;

            // Claims
            var name = claims.FindFirstValue(ClaimTypes.Name);
            var avatar = claims.FindFirstValue(AvatarClaim);

            // Universal id
            var uidText = claims.FindFirstValue(ClaimTypes.Upn);
            Guid? uid = Guid.TryParse(uidText, out var uidExact) ? uidExact : null;

            // Is corporate
            var corporate = StringUtils.TryParse<bool>(claims.FindFirstValue(CorporateClaim)).GetValueOrDefault();

            // Validate
            if (string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                user.Id,
                user.Organization,
                name, user.RoleValue,
                user.ClientIp,
                user.DeviceId,
                user.Language,
                user.Region,
                uid,
                corporate,
                connectionId)
            {
                Avatar = avatar
            };
        }

        private static (string? Name, string? Avatar, string? JsonData, Guid? Uid, bool Corporate) GetLocalData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Name"),
                data.Get("Avatar"),
                data.Get("JsonData"),
                data.Get<Guid>("Uid"),
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
        public static CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region, string? connectionId = null)
        {
            // Get data
            var (id, organization, role, deviceId) = GetData(data);
            var (name, avatar, jsonData, uid, corporate) = GetLocalData(data);

            // Validation
            if (id == null || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                id,
                organization,
                name,
                role.GetValueOrDefault(),
                ip,
                deviceId.GetValueOrDefault(),
                language,
                region,
                uid,
                corporate,
                connectionId)
            {
                Avatar = avatar,
                JsonData = jsonData
            };
        }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Connection id
        /// 链接编号
        /// </summary>
        public string? ConnectionId { get; }

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
        /// Universal id
        /// 全局编号
        /// </summary>
        public Guid? Uid { get; private set; }

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
        /// <param name="organization">Organization</param>
        /// <param name="name">Name</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="uid">Universal id</param>
        /// <param name="corporate">Is corporate</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, string? organization, string name, short roleValue, IPAddress clientIp, int deviceId, CultureInfo language, string region, Guid? uid = null, bool? corporate = null, string? connectionId = null)
            : base(id, organization, roleValue, clientIp, deviceId, language, region)
        {
            Name = name;
            Uid = uid;
            Corporate = corporate.GetValueOrDefault();
            ConnectionId = connectionId;
        }

        private IEnumerable<Claim> GetClaims()
        {
            yield return new(ClaimTypes.Name, Name);
            yield return new(CorporateClaim, Corporate.ToJson());

            if (Avatar != null)
                yield return new(AvatarClaim, Avatar);
            if (!string.IsNullOrEmpty(JsonData))
                yield return new(ClaimTypes.UserData, JsonData);
            if (Uid != null)
                yield return new(ClaimTypes.Upn, Uid.Value.ToString());
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
            var (name, avatar, jsonData, uid, corporate) = GetLocalData(data);

            // Name
            if (name != null)
                Name = name;

            // Corporate
            Corporate = corporate;

            // Avatar
            Avatar = avatar;

            // Uid
            Uid = uid;

            // Json data
            JsonData = jsonData;
        }
    }
}
