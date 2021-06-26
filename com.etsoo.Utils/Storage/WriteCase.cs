namespace com.etsoo.Utils.Storage
{
    public enum WriteCase
    {
        /// <summary>
        /// Appending to existing file
        /// 附加到现有文件
        /// </summary>
        Appending,

        /// <summary>
        /// Create new file
        /// 创建新文件
        /// </summary>
        CreateNew,

        /// <summary>
        /// Create new file or overwrite existing file
        /// 创建新文件或者覆盖现有文件
        /// </summary>
        CreateOrOverwrite
    }
}
