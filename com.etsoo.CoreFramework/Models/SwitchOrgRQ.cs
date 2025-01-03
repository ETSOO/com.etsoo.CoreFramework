using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Switch organization request data
    /// 切换机构请求数据
    /// </summary>
    public record SwitchOrgRQ : IModelValidator
    {
        /// <summary>
        /// Target organization id
        /// 目标机构编号
        /// </summary>
        public required int OrganizationId { get; init; }

        /// <summary>
        /// From organization id
        /// 来源机构编号
        /// </summary>
        public int? FromOrganizationId { get; init; }

        /// <summary>
        /// Core system access token
        /// 核心系统访问令牌
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public IActionResult? Validate()
        {
            if (Token.Length is not (>= 512 and <= 5120))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Token));
            }

            return null;
        }
    }
}
