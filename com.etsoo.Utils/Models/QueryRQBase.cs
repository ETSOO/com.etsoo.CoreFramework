using com.etsoo.Utils.Actions;

namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Query request data interface
    /// 查询请求数据接口
    /// </summary>
    public interface IQueryRQBase : IModelValidator
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        object? Id { get; }

        /// <summary>
        /// Is enabled or not
        /// 是否启用
        /// </summary>
        bool? Enabled { get; set; }

        /// <summary>
        /// Filter keyword
        /// 过滤关键字
        /// </summary>
        string? Keyword { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        QueryPagingData? QueryPaging { get; set; }
    }

    /// <summary>
    /// Query request data common base
    /// 查询请求数据通用基类
    /// </summary>
    public abstract record QueryRQBaseCommon : IQueryRQBase
    {
        object? IQueryRQBase.Id => GetId();

        /// <summary>
        /// Get id
        /// 获取编号
        /// </summary>
        /// <returns>Id</returns>
        protected abstract object? GetId();

        /// <summary>
        /// Enabled or not, null for all, true for enabled (<= EntityStatus.Approved), false for disabled (> 100)
        /// 是否启用
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Keyword to filter
        /// 用于过滤的关键字
        /// </summary>
        public virtual string? Keyword { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        public QueryPagingData? QueryPaging { get; set; }

        /// <summary>
        /// Generate result for a field
        /// 通过字段生成结果
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns>Action result</returns>
        protected virtual IActionResult? GenerateResult(string field)
        {
            return new ActionResult
            {
                Type = "NoValidData",
                Field = field
            };
        }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (!QueryPaging.IsOrderByValid())
                return GenerateResult(nameof(QueryPaging));

            if (Keyword?.Length > 128)
                return GenerateResult(nameof(Keyword));

            return null;
        }
    }

    /// <summary>
    /// Query request data base
    /// 查询请求数据基类
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public abstract record QueryRQBase<T> : QueryRQBaseCommon where T : struct
    {
        protected override object? GetId() => Id;

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T? Id { get; set; }

        /// <summary>
        /// Ids
        /// 编号列表
        /// </summary>
        public virtual IEnumerable<T>? Ids { get; set; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public virtual IEnumerable<T>? ExcludedIds { get; set; }
    }

    /// <summary>
    /// Query request data base
    /// 查询请求数据基类
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public abstract record QueryRQBase : QueryRQBaseCommon
    {
        protected override object? GetId() => Id;

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Ids
        /// 编号列表
        /// </summary>
        public virtual IEnumerable<string>? Ids { get; set; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public virtual IEnumerable<string>? ExcludedIds { get; set; }

        /// <summary>
        /// Is valid id or not
        /// 编号是否有效
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Result</returns>
        protected virtual bool IsValidId(string id)
        {
            return id.Length is (>= 1 and <= 256);
        }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public override IActionResult? Validate()
        {
            var result = base.Validate();
            if (result != null)
            {
                return result;
            }

            if (Id != null && !IsValidId(Id))
            {
                return GenerateResult(nameof(Id));
            }

            if (Ids != null && !Ids.All(IsValidId))
            {
                return GenerateResult(nameof(Ids));
            }

            if (ExcludedIds != null && !ExcludedIds.All(IsValidId))
            {
                return GenerateResult(nameof(ExcludedIds));
            }

            return null;
        }
    }
}
