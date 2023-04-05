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
        public int Id { get; set; }

        [JsonRequired]
        public string Title { get; set; } = string.Empty;
    }
}