namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / title item
    /// 编号 / 标题项目
    /// </summary>
    public record IdTitleItem
    {
        public required int Id { get; init; }

        public required string Title { get; init; }
    }
}