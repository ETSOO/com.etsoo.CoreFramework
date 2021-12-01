namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user interface
    /// 当前用户接口
    /// </summary>
    public interface ICurrentUser : IServiceUser
    {
        /// <summary>
        /// Unique connection id
        /// 唯一连接编号
        /// </summary>
        string? ConnectionId { get; }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        string? OrganizationName { get; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        string? Avatar { get; }

        /// <summary>
        /// Universal id
        /// 通用编号
        /// </summary>
        Guid? Uid { get; }

        /// <summary>
        /// Json data
        /// Json数据
        /// </summary>
        string? JsonData { get; set; }

        /// <summary>
        /// Is Corporate
        /// 是否为法人
        /// </summary>
        bool Corporate { get; }
    }
}