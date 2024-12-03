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
        /// <param name="idSelector">Id selector</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsoo<TSource, TKey>(this IQueryable<TSource> source, QueryRQ<TKey> rq, Expression<Func<TSource, TKey>> idSelector) where TKey : struct
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

            var p = rq.QueryPaging;
            if (p != null)
            {
                // Default order by
                p.OrderBy ??= [];

                // Avoid the id field with default prefix
                var idField = string.Join('.', idSelector.Body.ToString().Split('.').Skip(1));

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

                source = source.QueryEtsooPaging(p);
            }

            return source;
        }

        /// <summary>
        /// Etsoo query
        /// 亿速思维查询
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="rq">Query request data</param>
        /// <param name="idSelector">Id selector</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsoo<TSource>(this IQueryable<TSource> source, QueryRQ rq, Expression<Func<TSource, string>> idSelector)
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

            var p = rq.QueryPaging;
            if (p != null)
            {
                source = source.QueryEtsooPaging(p);
            }

            return source;
        }

        /// <summary>
        /// Etsoo query with string id
        /// 亿速思维查询（字符串编号）
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <typeparam name="TKey">Generic key type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="rq">Query request data</param>
        /// <param name="idSelector">Id selector</param>
        /// <param name="statusSelector">Status selector</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsoo<TSource, TKey>(this IQueryable<TSource> source, QueryRQ<TKey> rq, Expression<Func<TSource, TKey>> idSelector, Expression<Func<TSource, EntityStatus>> statusSelector) where TKey : struct
        {
            source = source.QueryEtsoo(rq, idSelector);

            if (rq.Status != null)
            {
                source = source.QueryEtsooEqual(rq.Status.Value, statusSelector.Body, statusSelector.Parameters[0]);
            }

            if (rq.Disabled != null)
            {
                // Create a constant expression representing the value to compare against
                var value = (byte)EntityStatus.Approved;
                var constant = Expression.Constant(value);

                // Create a binary expression representing the equality comparison
                var typeConvertBody = Expression.Convert(statusSelector.Body, value.GetType());
                var body = rq.Disabled.Value ? Expression.GreaterThan(typeConvertBody, constant) : Expression.LessThanOrEqual(typeConvertBody, constant);

                // Create a lambda expression representing the predicate
                var predicate = Expression.Lambda<Func<TSource, bool>>(body, statusSelector.Parameters[0]);

                // Call the Where method with the constructed predicate
                source = source.Where(predicate);
            }

            return source;
        }

        /// <summary>
        /// Etsoo query with string id
        /// 亿速思维查询（字符串编号）
        /// </summary>
        /// <typeparam name="TSource">Generic source type</typeparam>
        /// <param name="source">Source</param>
        /// <param name="rq">Query request data</param>
        /// <param name="idSelector">Id selector</param>
        /// <param name="statusSelector">Status selector</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("Expression requires dynamic code")]
        [RequiresUnreferencedCode("Expression requires unreferenced code")]
        public static IQueryable<TSource> QueryEtsoo<TSource>(this IQueryable<TSource> source, QueryRQ rq, Expression<Func<TSource, string>> idSelector, Expression<Func<TSource, EntityStatus>> statusSelector)
        {
            source = source.QueryEtsoo(rq, idSelector);

            if (rq.Status != null)
            {
                source = source.QueryEtsooEqual(rq.Status.Value, statusSelector.Body, statusSelector.Parameters[0]);
            }

            if (rq.Disabled != null)
            {
                // Create a constant expression representing the value to compare against
                var value = (byte)EntityStatus.Approved;
                var constant = Expression.Constant(value);

                // Create a binary expression representing the equality comparison
                var typeConvertBody = Expression.Convert(statusSelector.Body, value.GetType());
                var body = rq.Disabled.Value ? Expression.GreaterThan(typeConvertBody, constant) : Expression.LessThanOrEqual(typeConvertBody, constant);

                // Create a lambda expression representing the predicate
                var predicate = Expression.Lambda<Func<TSource, bool>>(body, statusSelector.Parameters[0]);

                // Call the Where method with the constructed predicate
                source = source.Where(predicate);
            }

            return source;
        }
    }
}
