using com.etsoo.CoreFramework.Application;
using com.etsoo.Database;
using System.ComponentModel.DataAnnotations;
using System.Data;

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
        public ushort? Items { get; init; }

        /// <summary>
        /// Get parameters
        /// 获取参数集合
        /// </summary>
        /// <returns></returns>
        public virtual IDbParameters AsParameters(ICoreApplicationBase app)
        {
            var parameters = new DbParameters();

            app.DB.AddParameter(parameters, nameof(Id), Id, DatabaseUtils.TypeToDbType(typeof(T)).GetValueOrDefault());

            if (ExcludedIds != null)
            {
                var idParameter = app.DB.ListToParameter(ExcludedIds, null, (type) => SqlServerUtils.GetListCommand(type, app.BuildCommandName));
                parameters.Add(nameof(ExcludedIds), idParameter);
            }
            else
            {
                parameters.Add(nameof(ExcludedIds), null);
            }

            parameters.Add(nameof(Keyword), Keyword?.ToDbString(false, 50));

            app.DB.AddParameter(parameters, nameof(Items), Items, DbType.UInt16);

            return parameters;
        }
    }

    /// <summary>
    /// Tiplist Request data with string id
    /// 动态列表请求数据
    /// </summary>
    public record TiplistRQ : IModelParameters
    {
        /// <summary>
        /// Current id
        /// 当前编号
        /// </summary>
        public string? Id { get; init; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public IEnumerable<string>? ExcludedIds { get; set; }

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
        public ushort? Items { get; init; }

        /// <summary>
        /// Get parameters
        /// 获取参数集合
        /// </summary>
        /// <returns></returns>
        public virtual IDbParameters AsParameters(ICoreApplicationBase app)
        {
            var parameters = new DbParameters();

            parameters.Add(nameof(Id), Id?.ToDbString(true));

            if (ExcludedIds != null)
            {
                var idParameter = app.DB.ListToParameter(ExcludedIds, DbType.String, null, (type) => SqlServerUtils.GetListCommand(type, app.BuildCommandName));
                parameters.Add(nameof(ExcludedIds), idParameter);
            }
            else
            {
                parameters.Add(nameof(ExcludedIds), null);
            }

            parameters.Add(nameof(Keyword), Keyword?.ToDbString(false, 50));

            app.DB.AddParameter(parameters, nameof(Items), Items, DbType.UInt16);

            return parameters;
        }
    }
}
