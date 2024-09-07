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
        Supplier = 4,
    }

    /// <summary>
    /// Identity type with flags
    /// 标识类型带标志
    /// </summary>
    [Flags]
    public enum IdentityTypeFlags : byte
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
        Supplier = 4,
    }
}
