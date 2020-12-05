using com.etsoo.CoreFramework.ActionResult;
using System;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application constant errors
    /// 程序错误常量
    /// </summary>
    public static class ApplicationErrors
    {
        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ActionResultNoData NoActionResult => CreateError(Resources.Resource.NoActionResult, "NoActionResult");

        /// <summary>
        /// Create error
        /// 创建错误
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="type">Type</param>
        /// <returns>Error</returns>
        public static ActionResultNoData CreateError(string title, string type)
        {
            return new ActionResultNoData()
            {
                Type = new Uri("https://api.etsoo.com/SmartERPNext/errors/" + type),
                Title = title
            };
        }
    }
}
