namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action data result
    /// 操作数据结果
    /// </summary>
    /// <typeparam name="D">Generic data type</typeparam>
    public record ActionDataResult<D>
    {
        /// <summary>
        /// Action result
        /// 操作结果
        /// </summary>
        public readonly IActionResult Result;

        /// <summary>
        /// Result data
        /// 结果数据
        /// </summary>
        public readonly D Data;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="result">Action result</param>
        /// <param name="data">Result data</param>
        public ActionDataResult(IActionResult result, D data)
        {
            Result = result;
            Data = data;
        }

        /// <summary>
        /// Merge result data to action result
        /// 合并结果数据到操作结果中
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public async ValueTask<IActionResult> MergeDataAsync()
        {
            if (Data is IDictionary<string, object> dic)
            {
            }
            else if (Data is object obj)
            {
                // For performance consideration, Data implements IJsonSerialization
                dic = await SharedUtils.ObjectToDictionaryAsync(obj);
            }
            else
            {
                throw new NotImplementedException();
            }

            foreach (var dicItem in dic)
            {
                Result.Data[dicItem.Key] = dicItem.Value;
            }

            return Result;
        }

        /// <summary>
        /// Merge result data to action result, simple way
        /// 合并结果数据到操作结果中，简单思路
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public IActionResult MergeData(string fieldName)
        {
            Result.Data[fieldName] = Data;
            return Result;
        }
    }
}
