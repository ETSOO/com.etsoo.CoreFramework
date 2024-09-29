using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Public service user data
    /// 服务用户公共数据
    /// </summary>
    public record PublicServiceUserData : PublicUserData
    {
        /// <summary>
        /// Service passphrase
        /// 服务口令
        /// </summary>
        public required string Passphrase { get; init; }

        /// <summary>
        /// User global unique identifier
        /// 用户全局唯一标识
        /// </summary>
        public string? Uid { get; init; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        public string? OrganizationName { get; init; }

        public override void SaveTo(IActionResult result)
        {
            base.SaveTo(result);

            result.Data[nameof(Passphrase)] = Passphrase;
            result.Data[nameof(Uid)] = Uid;
            result.Data[nameof(OrganizationName)] = OrganizationName;
        }
    }
}
