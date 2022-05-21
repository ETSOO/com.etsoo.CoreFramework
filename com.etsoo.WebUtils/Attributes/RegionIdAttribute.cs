using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace com.etsoo.WebUtils.Attributes
{
    /// <summary>
    /// Country or region id validation attribute
    /// 国家或地区标识验证属性
    /// </summary>
    public class RegionIdAttribute : ValidationAttribute
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

            try
            {
                _=new RegionInfo(valueAsString);
                return true;
            }
            catch
            {
                return false;
            }
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
