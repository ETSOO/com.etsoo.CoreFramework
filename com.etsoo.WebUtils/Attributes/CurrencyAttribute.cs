using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace com.etsoo.WebUtils.Attributes
{
    /// <summary>
    /// Currency code validation attribute
    /// 货币代码验证属性
    /// </summary>
    public partial class CurrencyAttribute : ValidationAttribute
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

            // CNY, USD
            return MyRegex().IsMatch(valueAsString);
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

        [GeneratedRegex("^[A-Z]{3}$")]
        private static partial Regex MyRegex();
    }
}
