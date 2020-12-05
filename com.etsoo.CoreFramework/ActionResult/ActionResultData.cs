namespace com.etsoo.CoreFramework.ActionResult
{
    /// <summary>
    /// Action result empty data
    /// 操作结果空数据
    /// </summary>
    public class ActionResultEmptyData
    {
    }

    /// <summary>
    /// Action result with empty failure data interface
    /// 没有错误数据的操作结果接口
    /// </summary>
    /// <typeparam name="T">Generic success data type</typeparam>
    public interface IActionResultSuccessData<T> : IActionResult<T, ActionResultEmptyData>
    {
    }

    /// <summary>
    /// Action result with empty failure data
    /// 没有错误数据的操作结果
    /// </summary>
    /// <typeparam name="T">Generic success data type</typeparam>
    public class ActionResultSuccessData<T> : ActionResult<T, ActionResultEmptyData>, IActionResultSuccessData<T>
    {
    }

    /// <summary>
    /// Action result with empty success and failure data interface
    /// 没有成功和错误数据的操作结果接口
    /// </summary>
    public interface IActionResultNoData : IActionResultSuccessData<ActionResultEmptyData>
    {
    }

    /// <summary>
    /// Action result with empty success and failure data
    /// 没有成功和错误数据的操作结果
    /// </summary>
    public class ActionResultNoData : ActionResultSuccessData<ActionResultEmptyData>, IActionResultNoData
    {
    }
}