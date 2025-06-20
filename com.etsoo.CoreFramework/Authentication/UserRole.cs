﻿namespace com.etsoo.CoreFramework.Authentication
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
        /// Team leader
        /// 团队负责人
        /// </summary>
        Leader = 32,

        /// <summary>
        /// Manager
        /// 经理
        /// </summary>
        Manager = 64,

        /// <summary>
        /// HR Manager
        /// 人事经理
        /// </summary>
        HRManager = 128,

        /// <summary>
        /// Finance
        /// 财务
        /// </summary>
        Finance = 256,

        /// <summary>
        /// Director
        /// 总监
        /// </summary>
        Director = 512,

        /// <summary>
        /// Executives
        /// 高管
        /// </summary>
        Executive = 1024,

        /// <summary>
        /// API
        /// 接口
        /// </summary>
        API = 4096,

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
