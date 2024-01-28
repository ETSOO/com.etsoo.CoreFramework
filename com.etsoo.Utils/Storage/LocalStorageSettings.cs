namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Local Storage settings
    /// 本地存储设置
    /// </summary>
    public record LocalStorageSettings
    {
        /// <summary>
        /// Root
        /// 根目录
        /// </summary>
        public string? Root { get; init; }

        /// <summary>
        /// URL root
        /// URL根路径
        /// </summary>
        public string? URLRoot { get; init; }
    }
}