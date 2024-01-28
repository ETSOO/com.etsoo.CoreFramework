using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

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
        /// <returns>Result</returns>
        [RequiresDynamicCode("MergeDataAsync 'Data' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("MergeDataAsync 'Data' may require dynamic access otherwise can break functionality when trimming application code")]
        public async ValueTask<IActionResult> MergeDataAsync()
        {
            if (Data is IDictionary<string, object> dic)
            {
            }
            else
            {
                // For performance consideration, Data implements IJsonSerialization
                dic = await SharedUtils.ObjectToDictionaryAsync(Data);
            }

            foreach (var dicItem in dic)
            {
                Result.Data[dicItem.Key] = dicItem.Value;
            }

            return Result;
        }

        /// <summary>
        /// Merge result data to action result
        /// 合并结果数据到操作结果中
        /// </summary>
        /// <param name="typeInfo">Json type info</param>
        /// <returns>Result</returns>
        public async ValueTask<IActionResult> MergeDataAsync(JsonTypeInfo<D> typeInfo)
        {
            if (Data is IDictionary<string, object> dic)
            {
            }
            else
            {
                // For performance consideration, Data implements IJsonSerialization
                dic = await SharedUtils.ObjectToDictionaryAsync(Data, typeInfo);
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
        /// <param name="fieldName">New data store field</param>
        /// <returns>Result</returns>
        public IActionResult MergeData(string fieldName)
        {
            Result.Data[fieldName] = Data;
            return Result;
        }
    }
}
