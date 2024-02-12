namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Quick update configs
    /// 快速更新配置
    /// </summary>
    public record QuickUpdateConfigs
    {
        /// <summary>
        /// Conditions
        /// 限制条件
        /// </summary>
        public string? Conditions { get; init; }

        /// <summary>
        /// Id field name
        /// 编号字段名称
        /// </summary>
        public string IdField { get; init; } = "Id";

        /// <summary>
        /// Table name, default is entity name
        /// 表格名称，默认为实体名称
        /// </summary>
        public string? TableName { get; init; }

        /// <summary>
        /// Updatable fields, should keep code side field name and table field name the same
        /// 支持更新的字段，必须确保代码端模块字段名称和表格字段名称一样
        /// </summary>
        public IEnumerable<string> UpdatableFields { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="updatableFields">Updatable fields</param>
        public QuickUpdateConfigs(IEnumerable<string> updatableFields)
        {
            UpdatableFields = updatableFields;
        }
    }
}
