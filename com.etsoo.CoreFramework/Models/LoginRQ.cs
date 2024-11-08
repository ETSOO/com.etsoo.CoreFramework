using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Login request data
    /// 登录请求数据
    /// </summary>
    public record LoginRQ : LoginIdRQ
    {
        /// <summary>
        /// Login password
        /// 登录密码
        /// </summary>
        public required string Pwd { get; init; }

        /// <summary>
        /// Timezone name
        /// 时区名称
        /// </summary>
        public string? Timezone { get; init; }

        /// <summary>
        /// Authentication request
        /// 授权请求
        /// </summary>
        public AuthRequest? Auth { get; init; }

        public override IActionResult? Validate()
        {
            var result = base.Validate();
            if (result != null)
            {
                return result;
            }

            if (Pwd.Length is not (>= 64 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Pwd));
            }

            return null;
        }
    }
}
