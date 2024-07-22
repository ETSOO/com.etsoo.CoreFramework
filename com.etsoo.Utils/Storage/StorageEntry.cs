namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Storage entry
    /// 存储条目
    /// </summary>
    public record StorageEntry
    {
        /// <summary>
        /// Name
        /// 名称
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Full path of the file or directory
        /// 文件或目录的完整路径
        /// </summary>
        public required string FullName { get; init; }

        /// <summary>
        /// Is file or not
        /// 是否为文件
        /// </summary>
        public bool IsFile { get; init; }

        /// <summary>
        /// File size
        /// 文件大小
        /// </summary>
        public long? Size { get; init; }

        /// <summary>
        /// Creation time
        /// 创建时间
        /// </summary>
        public required DateTime CreationTime { get; init; }

        /// <summary>
        /// Last write time
        /// 上次写入时间
        /// </summary>
        public required DateTime LastWriteTime { get; init; }
    }
}
