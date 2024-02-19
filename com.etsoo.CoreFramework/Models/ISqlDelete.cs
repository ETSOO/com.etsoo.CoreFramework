using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// SQL delete interface
    /// SQL删除接口
    /// </summary>
    public interface ISqlDelete
    {
        /// <summary>
        /// Create SQL delete command
        /// 创建SQL删除命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlDelete(IDatabase db);
    }
}