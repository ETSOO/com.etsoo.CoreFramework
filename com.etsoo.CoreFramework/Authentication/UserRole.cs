namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Standard User Roles
    /// 标准用户角色
    /// </summary>
    [Flags]
    public enum UserRole : short
    {
        /// <summary>
        /// Guest
        /// 访客
        /// </summary>
        Guest = 1,

        /// <summary>
        /// Outsourcing
        /// 外包
        /// </summary>
        Outsourcing = 2,

        /// <summary>
        /// Operator
        /// 操作员
        /// </summary>
        Operator = 4,

        /// <summary>
        /// Partner
        /// 渠道合作伙伴
        /// </summary>
        Partner = 8,

        /// <summary>
        /// User
        /// 用户
        /// </summary>
        User = 16,

        /// <summary>
        /// Cashier
        /// 出纳
        /// </summary>
        Cashier = 32,

        /// <summary>
        /// Team leader
        /// 团队负责人
        /// </summary>
        Leader = 64,

        /// <summary>
        /// Manager
        /// 经理
        /// </summary>
        Manager = 128,

        /// <summary>
        /// Finance
        /// 财务
        /// </summary>
        Finance = 256,

        /// <summary>
        /// HR Manager
        /// 人事经理
        /// </summary>
        HRManager = 512,

        /// <summary>
        /// Administrator
        /// 管理员
        /// </summary>
        Admin = 8192,

        /// <summary>
        /// Founder, takes all ownership
        /// 创始人，所有权限
        /// </summary>
        Founder = 16384
    }
}
