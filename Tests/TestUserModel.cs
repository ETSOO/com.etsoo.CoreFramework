using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using com.etsoo.SourceGenerators.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests
{
    public interface IUserData
    {
        public int Test { get; set; }
    }

    [AutoDataReaderGenerator]
    [AutoDictionaryGenerator]
    [AutoToJson]
    [AutoToParameters]
    public partial record DirectUser : QueryRQ<int>, IUserData
    {
        public string Name { get; init; } = null!;

        public readonly int ReadOnlyField;

        public int Test { get; set; }

        public bool WriteOnlyProperty { set { } }
    }

    [AutoToParameters]
    public partial record UpdateUser : UpdateModel<int>
    {
        // Name
        public string? Name { get; init; }

        protected async Task<bool> SetupAsync(IDbParameters parameters, ICoreApplicationBase app)
        {
            await Task.CompletedTask;
            throw new Exception("Validation failed");
        }
    }

    [AutoToParameters]
    public partial record UserUpdateModule : UpdateModel<int>
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
    public partial record TestUserModule
    {
        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public static string Type => "Test";

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