using com.etsoo.Utils.Models;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Attributes
{
    /// <summary>
    /// Country id validation attribute
    /// 国家标识验证属性
    /// </summary>
    public class CountryIdAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public CountryIdAttribute()
            : base(Properties.Resources.CountryIdAttributeError)
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

            return GlobalData.CountryIds.Contains(valueAsString);
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
