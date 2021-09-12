using com.etsoo.CoreFramework.Models;
using com.etsoo.SourceGenerators.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests
{
    [AutoDataReaderGenerator]
    [AutoDictionaryGenerator]
    [AutoToJson]
    [AutoToParameters]
    public sealed partial record DirectUser : QueryRQ
    {
        public int? Id { get; init; }
        public string Name { get; init; } = null!;
    }

    public sealed partial record UserUpdateModule : UpdateModel<int>
    {
        public string? Name { get; init; }
    }

    /// <summary>
    /// User module for test
    /// </summary>
    [Table("User")]
    [AutoDataReaderGenerator]
    [AutoDictionaryGenerator]
    [AutoToParameters]
    public sealed partial record TestUserModule
    {
        /// <summary>
        /// Status enum
        /// 状态枚举
        /// </summary>
        public enum StatusEnum : byte
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
        public int Id { get; init; }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(128)]
        public string? Name { get; init; }

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        public StatusEnum? Status { get; init; }

        /// <summary>
        /// Friends
        /// 朋友
        /// </summary>
        [NotMapped]
        [ArrayProperty('\n')]
        public int[]? Friends { get; init; }

        /// <summary>
        /// Keys
        /// </summary>
        [NotMapped]
        [Property(TypeName = "et_int_dic")]
        public Dictionary<int, int>? Keys { get; init; }
    }
}