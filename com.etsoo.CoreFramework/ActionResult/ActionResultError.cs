namespace com.etsoo.CoreFramework.ActionResult
{
    /// <summary>
    /// Action result error
    /// 操作结果错误
    /// </summary>
    public record ActionResultError
    {
        /// <summary>
        /// Name
        /// 名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Reason
        /// 原因
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Construct
        /// 构造函数
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="reason">Reason</param>
        public ActionResultError(string name, string reason) => (Name, Reason) = (name, reason);
    }
}
