using com.etsoo.CoreFramework.Authentication;
using com.etsoo.CoreFramework.Business;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Constants
    /// 常量
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Content-Disposition header item name
        /// </summary>
        public const string ContentDispositionHeaderName = "Content-Disposition";

        /// <summary>
        /// Refresh token header item name
        /// Keep the same with AuthApi.HeaderTokenField@etsoo/appscript
        /// </summary>
        public const string RefreshTokenHeaderName = "Etsoo-Refresh-Token";

        /// <summary>
        /// Current user id parameter name
        /// </summary>
        public const string CurrentUserField = "CurrentUser";

        /// <summary>
        /// Current organization id parameter name
        /// </summary>
        public const string CurrentOrgField = "CurrentOrg";

        /// <summary>
        /// Admin users roles
        /// </summary>
        public const UserRole AdminRoles = UserRole.Executive | UserRole.Admin | UserRole.Founder;

        /// <summary>
        /// Finance users roles
        /// </summary>
        public const UserRole FinanceRoles = UserRole.Finance | AdminRoles;

        /// <summary>
        /// HR users roles
        /// </summary>
        public const UserRole HRRoles = UserRole.HRManager | AdminRoles;

        /// <summary>
        /// Manager users roles
        /// </summary>
        public const UserRole ManagerRoles = UserRole.Manager | UserRole.HRManager | UserRole.Director | UserRole.API | FinanceRoles;

        /// <summary>
        /// User roles
        /// </summary>
        public const UserRole UserRoles = UserRole.User | UserRole.Leader | ManagerRoles;

        /// <summary>
        /// Determines whether the specified product unit represents an asset-based unit
        /// 确定指定的产品单位是否是资产的单位
        /// </summary>
        /// <param name="unit">The product unit to evaluate</param>
        /// <returns>result</returns>
        public static bool IsAssetUnit(ProductUnit unit)
        {
            return unit == ProductUnit.TIME
                || unit == ProductUnit.MONEY
                || (unit >= ProductUnit.HOUR && unit <= ProductUnit.HALFYEAR);
        }
    }
}
