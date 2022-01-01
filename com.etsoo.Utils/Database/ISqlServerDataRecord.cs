using Microsoft.Data.SqlClient.Server;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Sqlserver DataRecord support interface
    /// </summary>
    public interface ISqlServerDataRecord
    {
        /// <summary>
        /// Create SqlDataRecord
        /// 创建 SqlDataRecord
        /// </summary>
        /// <returns>SqlDataRecord</returns>
        static abstract SqlDataRecord Create();

        /// <summary>
        /// TVP type name
        /// 表值参数类型名称
        /// </summary>
        static abstract string TypeName { get; }

        /// <summary>
        /// Set SqlDataRecord values
        /// 设置SqlDataRecord值
        /// </summary>
        /// <param name="sdr">SqlDataRecord</param>
        void SetValues(SqlDataRecord sdr);
    }
}
