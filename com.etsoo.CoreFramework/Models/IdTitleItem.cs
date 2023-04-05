using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / title item
    /// 编号 / 标题项目
    /// </summary>
    public record IdTitleItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    /// <summary>
    /// Id / title JSON context
    /// 编号 / 标题项目 JSON 上下文
    /// </summary>
    [JsonSerializable(typeof(IdTitleItem))]
    public partial class IdTitleContext : JsonSerializerContext
    {
    }
}
