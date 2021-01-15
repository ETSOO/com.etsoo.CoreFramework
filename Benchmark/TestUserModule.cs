using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Benchmark
{
    /// <summary>
    /// User module for test
    /// </summary>
    [Table("e_user")]
    public class TestUserModule
    {
        /// <summary>
        /// Status enum
        /// 状态枚举
        /// </summary>
        public enum StatusEnum
        {
            Normal,
            Deleted
        }

        /// <summary>
        /// Auto generated id
        /// 自动编号
        /// </summary>
        [Key]
        [Range(1001, 1010)]
        public int? Id { get; set; }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(128)]
        public string? Name { get; set; }

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        [NotMapped]
        public StatusEnum? Status { get; set; }
    }
}