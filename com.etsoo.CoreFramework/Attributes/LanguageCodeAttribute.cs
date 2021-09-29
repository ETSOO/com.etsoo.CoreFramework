using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace com.etsoo.CoreFramework.Attributes
{
    /// <summary>
    /// Language code validation attribute
    /// 语言代码验证属性
    /// </summary>
    public class LanguageCodeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public LanguageCodeAttribute()
            : base(Properties.Resources.LanguageCodeAttributeError)
        {
        }

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

            return Regex.IsMatch(valueAsString, "^[a-z]{2}(-[A-Z]{2})?$");
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
