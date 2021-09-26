using com.etsoo.SourceGenerators.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tests
{
    /// <summary>
    /// User model
    /// </summary>
    [AutoDataReaderGenerator(UtcDateTime = true)]
    [AutoDictionaryGenerator]
    [AutoToJson]
    [AutoToParameters(true, SnakeCase = false)]
    public partial struct UserModel
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
        public string? Name;

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        public StatusEnum? Status { get; init; }

        /// <summary>
        /// Friends
        /// 朋友
        /// </summary>
        [Property(TypeName = "et_int_ids")]
        public int[]? Friends { get; init; }

        /// <summary>
        /// List
        /// </summary>
        public List<int> List;

        public bool? Valid { get; set; }

        public short? ShortValue { get; init; }

        public float? FloatValue { get; init; }

        public decimal? DecimalValue { get; init; }

        public ushort UShortValue { get; init; }

        public DateTime? Date { get; init; }

        /// <summary>
        /// List
        /// </summary>
        public List<StatusEnum>? ListStatus { get; set; }

        /// <summary>
        /// Dictionary
        /// </summary>
        [Property(TypeName = "et_int_dic")]
        public Dictionary<string, int>? Keys { get; set; }
    }
}
