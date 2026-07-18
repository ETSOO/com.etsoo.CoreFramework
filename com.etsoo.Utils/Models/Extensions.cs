using System.Text.RegularExpressions;

namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Extensions
    /// 扩展
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Is the order by fields valid
        /// 排序字段是否有效
        /// </summary>
        /// <param name="data">Pagination data</param>
        /// <returns>Result</returns>
        public static bool IsOrderByValid(this QueryPagingData? data)
        {
            if (data == null || data.OrderBy == null) return true;
            return !data.OrderBy.Any(o => !IsValidField(o.Field));
        }

        /// <summary>
        /// Is the field valid
        /// 字段是否有效
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns>Result</returns>
        public static bool IsValidField(string field)
        {
            return OrderFieldRegex().IsMatch(field);
        }

        /// <summary>
        /// Get all none null items
        /// 获取所有非空项目
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="list">List</param>
        /// <returns>Result</returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> list) where T : notnull
        {
            foreach (var item in list)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        [GeneratedRegex("^[0-9a-zA-Z_\\.]+$")]
        private static partial Regex OrderFieldRegex();
    }
}
