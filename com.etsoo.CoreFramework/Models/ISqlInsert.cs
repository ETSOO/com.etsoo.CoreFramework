using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// SQL insert interface
    /// SQL插入接口
    /// </summary>
    public interface ISqlInsert
    {
        /// <summary>
        /// Create SQL insert command
        /// 创建SQL插入命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlInsert(IDatabase db);
    }
}
