namespace com.etsoo.CoreFramework.ActionResult
{
    /// <summary>
    /// Action result with items
    /// 带列表项目的操作结果
    /// </summary>
    /// <typeparam name="T">Generic success data type</typeparam>
    /// <typeparam name="TI">Generic success data items type</typeparam>
    /// <typeparam name="F">Generic failure data type</typeparam>
    /// <typeparam name="FI">Generic failure data items type</typeparam>
    public class ActionResultItems<T, TI, F, FI> : ActionResult<T, F>, IActionResultItems<T, TI, F, FI>
        where T : IActionResultDataItems<TI>
        where F : IActionResultDataItems<FI>
    {
    }
}