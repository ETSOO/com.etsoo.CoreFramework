﻿using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils;
using com.etsoo.Utils.String;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user data
    /// 服务用户数据
    /// </summary>
    public record ServiceUser : UserToken, IServiceUser
    {
        /// <summary>
        /// Role value claim type
        /// 角色值类型
        /// </summary>
        public const string RoleValueClaim = "rolevalue";

        /// <summary>
        /// Create user from claims
        /// 从声明创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>User</returns>
        public new static ServiceUser? Create(ClaimsPrincipal claims)
        {
            var token = UserToken.Create(claims);
            if (token == null) return null;

            // Claims
            var language = claims.FindFirstValue(ClaimTypes.Locality);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(RoleValueClaim)).GetValueOrDefault();
            var userUidString = claims.FindFirstValue(ClaimTypes.PrimarySid);

            // Validate
            if (string.IsNullOrEmpty(language))
                return null;

            // User Uid
            Guid? uid = Guid.TryParse(userUidString, out var userUidTemp) ? userUidTemp : null;

            // New user
            return new ServiceUser(
                token.Id,
                uid,
                token.Organization,
                roleValue,
                token.ClientIp,
                token.DeviceId,
                new CultureInfo(language),
                token.Region);
        }

        /// <summary>
        /// Get role from value
        /// 从值获取角色
        /// </summary>
        /// <param name="roleValue">Role value</param>
        /// <returns>User role</returns>
        public static UserRole? GetRole(short roleValue)
        {
            var userRole = (UserRole)roleValue;
            return SharedUtils.EnumIsDefined(userRole) ? userRole : null;
        }

        /// <summary>
        /// Get dictionary data
        /// 获取字典数据
        /// </summary>
        /// <param name="data">Input</param>
        /// <returns>Result</returns>
        static (string? id, string? organization, short? Role, int? deviceId, Guid? uid) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get<int>("DeviceId"),
                data.Get<Guid>("Uid")
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
        /// <returns>User</returns>
        public static ServiceUser? CreateFromData(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region)
        {
            // Get data
            var (id, organization, role, deviceId, uid) = GetData(data);

            // Validation
            if (id == null || role == null || deviceId == null)
                return null;

            // New user
            return new ServiceUser(
                id,
                uid,
                organization,
                role.Value,
                ip,
                deviceId.Value,
                language,
                region);
        }

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Service user</returns>
        public static ServiceUser? Create(HttpContext? context)
        {
            // https://stackoverflow.com/questions/32584074/whats-the-role-of-the-claimsprincipal-why-does-it-have-multiple-identities
            // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
            // Only concerns the main identity
            if (context == null || context.User == null)
                return null;

            return Create(context.User);
        }

        /// <summary>
        /// Create non nullable user
        /// 创建非空用户
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Service user</returns>
        public static ServiceUser CreateSafe(HttpContext? context)
        {
            var user = Create(context);
            return user ?? throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        public virtual short RoleValue { get; private set; }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        public UserRole? Role { get; private set; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        public CultureInfo Language { get; }

        /// <summary>
        /// User Uid
        /// 用户全局编号
        /// </summary>
        [NotNullIfNotNull(nameof(Organization))]
        public Guid? Uid { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="uid">Uid, shared between multiple applications</param>
        /// <param name="organization">Organization identifying units in SaSS</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        public ServiceUser(string id, Guid? uid, string? organization, short roleValue, IPAddress clientIp, int deviceId, CultureInfo language, string region)
            : base(id, clientIp, region, deviceId, organization)
        {
            RoleValue = roleValue;
            Role = GetRole(roleValue);

            Language = language;
            Uid = uid;
        }

        /// <summary>
        /// Constructor, for simple case
        /// 构造函数，适应于简单情形
        /// </summary>
        /// <param name="role">User role</param>
        /// <param name="id">Username</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="organization">Organization</param>
        /// <param name="uid">Uid, shared between multiple applications</param>
        /// <param name="deviceId">Device id</param>
        public ServiceUser(UserRole role, string id, IPAddress clientIp, CultureInfo language, string region, string? organization = null, Guid? uid = null, int deviceId = 0)
            : this(id, uid, organization, (short)role, clientIp, deviceId, language, region)
        {
        }

        private IEnumerable<Claim> GetClaims()
        {
            yield return new(ClaimTypes.Locality, Language.Name);
            yield return new(RoleValueClaim, RoleValue.ToString());
            if (Uid != null) yield return new(ClaimTypes.PrimarySid, Uid.Value.ToString());

            if (Role.HasValue)
            {
                var roles = Role.Value.GetKeys();

                // Multiple roles
                foreach (var role in roles)
                {
                    yield return new(ClaimTypes.Role, role);
                }
            }
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
        /// <param name="newRole">New role</param>
        public void Update(UserRole newRole)
        {
            if (!Role.Equals(newRole))
            {
                Role = newRole;
                RoleValue = (short)newRole;
            }
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        public virtual void Update(StringKeyDictionaryObject data)
        {
            // Editable fields
            var (_, _, role, _, _) = GetData(data);

            // Role
            if (role != null && RoleValue != role)
                RoleValue = role.Value;
        }
    }
}
