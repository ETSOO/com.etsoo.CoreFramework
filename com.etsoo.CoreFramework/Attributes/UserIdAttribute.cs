﻿using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Attributes
{
    /// <summary>
    /// User id validation attribute
    /// 用户标识验证属性
    /// </summary>
    public class UserIdAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public UserIdAttribute()
            : base(Properties.Resources.UserIdAttributeError)
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
            if (value is not string valueAsString)
            {
                return false;
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

        /// <summary>
        /// Override format error message
        /// 覆盖格式化错误信息
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Formated string</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, name);
        }
    }
}