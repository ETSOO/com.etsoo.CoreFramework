using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// SQL update interface
    /// SQL更新接口
    /// </summary>
    public interface ISqlUpdate : IUpdateModel
    {
        /// <summary>
        /// Create SQL update command
        /// 创建SQL更新命令
        /// </summary>
        /// <param name="db">Database</param>
        /// <returns>Result</returns>
        (string, IDbParameters) CreateSqlUpdate(IDatabase db);
    }
}
