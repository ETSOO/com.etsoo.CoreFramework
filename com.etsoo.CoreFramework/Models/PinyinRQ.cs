using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Business;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Get Pinyin request data
    /// 获取拼音请求数据
    /// </summary>
    public record PinyinRQ : IModelValidator
    {
        /// <summary>
        /// Input string
        /// 输入的字符串
        /// </summary>
        public required string Input { get; init; }

        /// <summary>
        /// Return format
        /// 返回格式
        /// </summary>
        public PinyinFormatType Format { get; init; }

        /// <summary>
        /// Is name
        /// 是否为姓名
        /// </summary>
        public bool? IsName { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (Input.Length is not (>= 1 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Input));
            }

            return null;
        }
    }
}
