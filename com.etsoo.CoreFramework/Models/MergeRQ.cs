using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Merget request data
    /// 合并请求数据
    /// </summary>
    public record MergeRQ
    {
        /// <summary>
        /// Source id
        /// 原编号
        /// </summary>
        [Required]
        public int SourceId { get; init; }

        /// <summary>
        /// Merge target id
        /// 合并目标编号
        /// </summary>
        [Required]
        public int TargetId { get; init; }

        /// <summary>
        /// Is to delete source item
        /// 是否删除原项目
        /// </summary>
        public bool? DeleteSource { get; init; }
    }
}
