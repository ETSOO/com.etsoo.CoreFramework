﻿using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Change password data
    /// 修改密码数据
    /// </summary>
    /// <param name="OldPassword">Current password</param>
    /// <param name="Password">New password</param>
    public record ChangePasswordDto(string OldPassword, string Password)
    {
        /// <summary>
        /// Signature
        /// 签名
        /// </summary>
        public string Sign { get; set; } = string.Empty;

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
                [nameof(OldPassword)] = OldPassword,
                [nameof(Password)] = Password
            };

            // With an extra '&' at the end
            var query = rq.JoinAsString().TrimEnd('&');

            return Convert.ToHexString(CryptographyUtils.HMACSHA256(query, appSecret));
        }
    }
}
