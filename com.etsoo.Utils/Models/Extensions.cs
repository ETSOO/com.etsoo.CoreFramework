namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Extensions
    /// 扩展
    /// </summary>
    public static class Extensions
    {
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
    }
}
