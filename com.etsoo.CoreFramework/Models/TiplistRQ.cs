using com.etsoo.CoreFramework.Application;
using com.etsoo.Database;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Tiplist Request data
    /// 动态列表请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record TiplistRQ<T> : IModelParameters where T : struct
    {
        /// <summary>
        /// Current id
        /// 当前编号
        /// </summary>
        public T? Id { get; init; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public IEnumerable<T>? ExcludedIds { get; set; }

        /// <summary>
        /// Search keyword
        /// 查询关键词
        /// </summary>
        public string? Keyword { get; init; }

        /// <summary>
        /// Maximum items
        /// 最大项目数
        /// </summary>
        [Range(1, 1000)]
        public uint? Items { get; init; }

        /// <summary>
        /// Get parameters
        /// 获取参数集合
        /// </summary>
        /// <returns></returns>
        public virtual DynamicParameters AsParameters(ICoreApplicationBase app)
        {
            var parameters = new DynamicParameters();

            parameters.Add("Id", Id);

            if (ExcludedIds != null)
            {
                var idParameter = app.DB.ListToParameter(ExcludedIds, null, (type) => SqlServerUtils.GetListCommand(type, app.BuildCommandName));
                parameters.Add("ExcludedIds", idParameter);
            }

            if (!string.IsNullOrEmpty(Keyword))
                parameters.Add("Keyword", new DbString { IsAnsi = false, IsFixedLength = false, Length = 50, Value = Keyword });
            parameters.Add("Items", Items);

            return parameters;
        }
    }

    /// <summary>
    /// Tiplist Request data with string id
    /// 动态列表请求数据
    /// </summary>
    public record TiplistStrRQ : TiplistRQ<int>
    {
        /// <summary>
        /// String id
        /// 字符串编号
        /// </summary>
        public string? Sid { get; init; }
    }
}
