using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Attributes
{
    /// <summary>
    /// Email address list
    /// 电子邮箱列表
    /// </summary>
    public class EmailListAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public EmailListAttribute()
            : base(Properties.Resources.EmailListAttributeError)
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

            if (value is not IEnumerable<string> values)
            {
                return false;
            }

            var emailAttribute = new EmailAddressAttribute();
            return values.All(email => emailAttribute.IsValid(email));
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
