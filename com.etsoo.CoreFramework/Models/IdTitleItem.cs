using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / title item
    /// 编号 / 标题项目
    /// </summary>
    public record IdTitleItem
    {
        [JsonRequired]
        public int Id { get; init; }

        [JsonRequired]
        public required string Title { get; init; }
    }
}