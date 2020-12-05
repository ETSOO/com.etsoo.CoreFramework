using System.Collections.Generic;

namespace com.etsoo.CoreFramework.ActionResult
{
    /// <summary>
    /// Action result with items data interface
    /// 操作结果含列表数据接口
    /// </summary>
    /// <typeparam name="T">Generic items type</typeparam>
    public interface IActionResultDataItems<T>
    {
        IEnumerable<T> Items { get; set; }
    }

    /// <summary>
    /// Action result with items interface
    /// 带列表项目的操作结果接口
    /// </summary>
    /// <typeparam name="T">Generic success data type</typeparam>
    /// <typeparam name="TI">Generic success data items type</typeparam>
    /// <typeparam name="F">Generic failure data type</typeparam>
    /// <typeparam name="FI">Generic failure data items type</typeparam>
    public interface IActionResultItems<T, TI, F, FI> : IActionResult<T, F>
        where T : IActionResultDataItems<TI>
        where F : IActionResultDataItems<FI>
    {
    }
}
