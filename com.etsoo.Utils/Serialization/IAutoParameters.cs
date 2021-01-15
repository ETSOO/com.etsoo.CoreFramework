using Dapper;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Auto created export parameters
    /// 自动创建的导出参数
    /// </summary>
    public interface IAutoParameters
    {
        /// <summary>
        /// Export parameters
        /// 导出参数
        /// </summary>
        /// <returns>Parameters</returns>
        DynamicParameters AsParameters();
    }
}