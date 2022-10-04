using com.etsoo.Utils.String;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.WebUtils.Attributes
{
    /// <summary>
    /// Is JSON validation attribute
    /// 是否为 JSON 数据验证属性
    /// </summary>
    public class IsJsonAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validity check
        /// 有效性检查
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Result</returns>
        public override bool IsValid(object? value)
        {
            if (value == null) return true;

            if (value is not string valueAsString)
            {
                return false;
            }

            return valueAsString.IsJson();
        }

        /// <summary>
        /// Override format error message
        /// 覆盖格式化错误信息
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Formated string</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }
    }
}
