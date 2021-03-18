using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// User id validation attribute
    /// 用户标识验证属性
    /// </summary>
    public class UserIdAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validity check
        /// 有效性检查
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Result</returns>
        public override bool IsValid(object? value)
        {
            if (value is not string valueAsString)
            {
                return true;
            }

            if (valueAsString.Contains('@'))
            {
                var email = new EmailAddressAttribute();
                return email.IsValid(value);
            }
            else
            {
                var phone = new PhoneAttribute();
                return phone.IsValid(value);
            }
        }
    }
}
