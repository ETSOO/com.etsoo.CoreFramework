using com.etsoo.CoreFramework.Application;
using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Export parameters model
    /// 导出参数模块
    /// </summary>
    public interface IModelParameters
    {
        /// <summary>
        /// Export parameters
        /// 导出参数
        /// </summary>
        /// <param name="app">Application</param>
        /// <returns>Parameters</returns>
        IDbParameters AsParameters(ICoreApplicationBase app);
    }
}
