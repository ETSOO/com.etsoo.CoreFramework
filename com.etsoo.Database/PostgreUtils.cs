using System.Text.RegularExpressions;

namespace com.etsoo.Database
{
    /// <summary>
    /// Postgre (Npg) database utilities
    /// Postgre (Npg) 数据库工具类
    /// </summary>
    public static class PostgreUtils
    {
        /// <summary>
        /// Convert IIF to CASE WHEN
        /// 转化IIF为CASE WHEN
        /// </summary>
        /// <param name="iifExpression">IIF expression</param>
        /// <returns>Result</returns>
        public static string ConvertIIFToCaseWhen(string iifExpression)
        {
            // Simple pattern only
            var pattern = @"IIF\(([^,]+),\s*([^,]+),\s*([^)]+)\)";

            var result = Regex.Replace(iifExpression, pattern, "CASE WHEN $1 THEN $2 ELSE $3 END");

            return result;
        }
    }
}
