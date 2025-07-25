using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace com.etsoo.WebUtils.Attributes
{
    /// <summary>
    /// Wechat ID validation attribute
    /// 微信编号验证属性
    /// </summary>
    public partial class WechatIdAttribute : ValidationAttribute
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

            // 正则：以字母开头，后面5到19个字符，可以是字母、数字、下划线、减号
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

        [GeneratedRegex(@"^[a-zA-Z][a-zA-Z0-9_-]{5,19}$")]
        private static partial Regex MyRegex();
    }
}
