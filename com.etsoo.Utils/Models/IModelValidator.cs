using com.etsoo.Utils.Actions;

namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Model validator interface, alternative to DataAnnotations
    /// 模块验证接口，替代DataAnnotations
    /// </summary>
    public interface IModelValidator
    {
        /// <summary>
        /// Validate the model, should static check only, null or success means passed
        /// 验证模块，仅静态检查，null or success表示通过
        /// </summary>
        /// <returns>Validation result</returns>
        IActionResult? Validate();
    }
}
