using com.etsoo.CoreFramework.Business;
using com.etsoo.Database;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model extentions
    /// 模块扩展
    /// </summary>
    public static class ModelExtentions
    {
        /// <summary>
        /// Etsoo query
        /// 亿速思维查询
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <typeparam name="TKey">Generic key type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="rq">Query request data</param>
        /// <param name="idSelector">Id field selector</param>
        /// <param name="statusSelector">Status field selector</param>
        /// <param name="otherFilters">Other filters</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsoo<TSource, TKey>(
            this IQueryable<TSource> source, QueryRQ<TKey> rq,
            Expression<Func<TSource, TKey>> idSelector,
            Expression<Func<TSource, EntityStatus>>? statusSelector = null,
            Func<IQueryable<TSource>, IQueryable<TSource>>? otherFilters = null) where TKey : struct
        {
            if (rq.Id.HasValue)
            {
                source = source.QueryEtsooEqual(rq.Id.Value, idSelector.Body, idSelector.Parameters[0]);
            }

            if (rq.Ids?.Count() > 0)
            {
                source = source.QueryEtsooContains(rq.Ids, idSelector);
            }

            if (rq.ExcludedIds?.Count() > 0)
            {
                source = source.QueryEtsooContains(rq.ExcludedIds, idSelector, true);
            }

            if (statusSelector != null)
            {
                source = source.QueryEtsooStatus(rq, statusSelector);
            }

            if (otherFilters != null)
            {
                source = otherFilters(source);
            }

            var p = rq.QueryPaging;
            if (p != null)
            {
                p.SetDefaultOrderBy(idSelector.Body);
                source = source.QueryEtsooPaging(p);
            }

            return source;
        }

        private static void SetDefaultOrderBy(this QueryPagingData p, Expression idBody)
        {
            // Default order by
            p.OrderBy ??= [];

            // Avoid the id field with default prefix
            var idField = string.Join('.', idBody.ToString().Split('.').Skip(1));

            // Get the id field
            var id = p.OrderBy.FirstOrDefault(o => o.Field.Equals(idField, StringComparison.OrdinalIgnoreCase));

            if (id == null)
            {
                p.OrderBy = p.OrderBy.Append(new() { Field = idField, Desc = true, Unique = true });
            }
            else
            {
                // Set unique to override initial value
                id.Unique = true;
            }
        }

        /// <summary>
        /// Etsoo query
        /// 亿速思维查询
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="rq">Query request data</param>
        /// <param name="idSelector">Id field selector</param>
        /// <param name="statusSelector">Status field selector</param>
        /// <param name="otherFilters">Other filters</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsoo<TSource>(
            this IQueryable<TSource> source,
            QueryRQ rq,
            Expression<Func<TSource, string>> idSelector,
            Expression<Func<TSource, EntityStatus>>? statusSelector = null,
            Func<IQueryable<TSource>, IQueryable<TSource>>? otherFilters = null)
        {
            if (!string.IsNullOrEmpty(rq.Id))
            {
                source = source.QueryEtsooEqual(rq.Id, idSelector.Body, idSelector.Parameters[0]);
            }

            if (rq.Ids?.Count() > 0)
            {
                source = source.QueryEtsooContains(rq.Ids, idSelector);
            }

            if (rq.ExcludedIds?.Count() > 0)
            {
                source = source.QueryEtsooContains(rq.ExcludedIds, idSelector, true);
            }

            if (statusSelector != null)
            {
                source = source.QueryEtsooStatus(rq, statusSelector);
            }

            if (otherFilters != null)
            {
                source = otherFilters(source);
            }

            var p = rq.QueryPaging;
            if (p != null)
            {
                p.SetDefaultOrderBy(idSelector.Body);
                source = source.QueryEtsooPaging(p);
            }

            return source;
        }

        /// <summary>
        /// Etsoo query with status
        /// Etsoo 状态查询
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="rq">Query request data</param>
        /// <param name="statusSelector">Status field selector</param>
        /// <returns>Result</returns>
        public static IQueryable<TSource> QueryEtsooStatus<TSource>(this IQueryable<TSource> source, IQueryRQ rq, Expression<Func<TSource, EntityStatus>> statusSelector)
        {
            if (rq.Status != null)
            {
                source = source.QueryEtsooEqual(rq.Status.Value, statusSelector.Body, statusSelector.Parameters[0]);
            }

            if (rq.Enabled != null)
            {
                // Create a constant expression representing the value to compare against
                var value = (byte)EntityStatus.Approved;
                var constant = Expression.Constant(value);

                // Create a binary expression representing the equality comparison
                var typeConvertBody = Expression.Convert(statusSelector.Body, value.GetType());
                var body = rq.Enabled.Value ? Expression.LessThanOrEqual(typeConvertBody, constant) : Expression.GreaterThan(typeConvertBody, constant);

                // Create a lambda expression representing the predicate
                var predicate = Expression.Lambda<Func<TSource, bool>>(body, statusSelector.Parameters[0]);

                // Call the Where method with the constructed predicate
                source = source.Where(predicate);
            }

            return source;
        }
    }
}
