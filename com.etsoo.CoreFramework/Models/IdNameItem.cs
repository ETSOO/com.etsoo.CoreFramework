using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / name item
    /// 'required' keyword not supported by JsonSerializerContext
    /// 编号 / 名称项目
    /// </summary>
    public record IdNameItem
    {
        [JsonRequired]
        public int Id { get; set; }

        [JsonRequired]
        public string Name { get; set; } = string.Empty;
    }
}
