namespace com.etsoo.CoreFramework.Business
{
    /// <summary>
    /// Identity type
    /// 标识类型
    /// </summary>
    public enum IdentityType : byte
    {
        /// <summary>
        /// User
        /// 用户
        /// </summary>
        User = 1,

        /// <summary>
        /// Customer
        /// 客户
        /// </summary>
        Customer = 2,

        /// <summary>
        /// Supplier
        /// 供应商
        /// </summary>
        Supplier = 4
    }

    /// <summary>
    /// Identity type with flags
    /// 标识类型带标志
    /// </summary>
    [Flags]
    public enum IdentityTypeFlags : byte
    {
        /// <summary>
        /// None
        /// 无标识
        /// </summary>
        None = 0,

        /// <summary>
        /// User
        /// 用户
        /// </summary>
        User = 1,

        /// <summary>
        /// Customer
        /// 客户
        /// </summary>
        Customer = 2,

        /// <summary>
        /// Supplier
        /// 供应商
        /// </summary>
        Supplier = 4,

        /// <summary>
        /// Contact
        /// 联系人
        /// </summary>
        Contact = 8,

        /// <summary>
        /// Organization
        /// 机构
        /// </summary>
        Org = 16,

        /// <summary>
        /// Department
        /// 部门
        /// </summary>
        Dept = 32
    }
}
