using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;

namespace com.etsoo.CoreFramework.DB
{
    /// <summary>
    /// Database utilities
    /// 数据库工具
    /// </summary>
    public static class DBUtils
    {
        /// <summary>
        /// Format parameters, extends DatabaseUtils.FormatParameters
        /// 格式化参数，扩展 DatabaseUtils.FormatParameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <param name="app">Application</param>
        /// <returns>Result</returns>
        public static IDbParameters FormatParameters(object parameters, ICoreApplicationBase app)
        {
            if (parameters is IModelParameters p)
            {
                return p.AsParameters(app);
            }

            return DatabaseUtils.FormatParameters(parameters) ?? new DbParameters();
        }
    }
}
