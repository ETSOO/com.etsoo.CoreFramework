using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model validation interface
    /// 模块自定义验证接口
    /// </summary>
    public interface IModelValidate
    {
        /// <summary>
        /// Model validation
        /// 模块自定义验证
        /// </summary>
        /// <param name="result">Action result</param>
        /// <returns>Full validated</returns>
        bool Validate(IActionResult result);
    }
}
