using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Change password request data
    /// 修改密码请求数据
    /// </summary>
    public record ChangePasswordRQ : IModelValidator
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public required string DeviceId { get; init; }

        /// <summary>
        /// Current password
        /// 当前密码
        /// </summary>
        public required string OldPassword { get; init; }

        /// <summary>
        /// New password
        /// 新密码
        /// </summary>
        public required string Password { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public IActionResult? Validate()
        {
            if (DeviceId.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(DeviceId));
            }

            if (OldPassword.Length is not (>= 64 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(OldPassword));
            }

            if (Password.Length is not (>= 64 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Password));
            }

            return null;
        }
    }
}
