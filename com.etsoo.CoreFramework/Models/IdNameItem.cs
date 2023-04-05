using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / name item
    /// 编号 / 名称项目
    /// </summary>
    public record IdNameItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Id / name JSON context
    /// 编号 / 名称项目 JSON 上下文
    /// </summary>
    [JsonSerializable(typeof(IdNameItem))]
    public partial class IdNameContext : JsonSerializerContext
    {
    }
}
