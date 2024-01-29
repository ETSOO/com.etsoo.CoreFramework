namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Id / name item
    /// 编号 / 名称项目
    /// </summary>
    public record IdNameItem
    {
        public required int Id { get; init; }

        public required string Name { get; init; }
    }
}
