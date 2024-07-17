using System.ComponentModel.DataAnnotations;

namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Storage options
    /// 存储选项
    /// </summary>
    public record StorageOptions
    {
        /// <summary>
        /// Root
        /// 根目录
        /// </summary>
        [Required]
        public string Root { get; set; } = string.Empty;

        /// <summary>
        /// URL root
        /// URL根路径
        /// </summary>
        [Required]
        [Url]
        public string URLRoot { get; set; } = string.Empty;
    }
}
