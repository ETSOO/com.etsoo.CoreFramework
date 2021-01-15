using com.etsoo.Utils.Actions;
using System;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application error
    /// 程序错误对象
    /// </summary>
    public record ApplicationError(Uri Type, string Title)
    {
        /// <summary>
        /// As AcionResult
        /// 输出为操作结果
        /// </summary>
        /// <returns>Action result</returns>
        public IActionResult AsResult()
        {
            return new ActionResult
            {
                Type = Type,
                Title = Title
            };
        }
    }

    /// <summary>
    /// Application errors
    /// 程序错误
    /// </summary>
    public static class ApplicationErrors
    {
        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoActionResult => new ApplicationError(new Uri("https://api.etsoo.com/SmartERPNext/errors/NoActionResult"), Resources.Resource.NoActionResult);
    }
}
