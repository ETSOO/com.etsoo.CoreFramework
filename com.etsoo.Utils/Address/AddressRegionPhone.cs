namespace com.etsoo.Utils.Address
{
    /// <summary>
    /// Country or region phone
    /// 国家或地区电话
    /// </summary>
    public partial record AddressRegion
    {
        /// <summary>
        /// Create phone
        /// 创建电话对象
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="regionId">Default country or egion id</param>
        /// <returns>Result</returns>
        public static Phone? CreatePhone(string phoneNumber, string? regionId = null)
        {
            // Remove empties
            phoneNumber = phoneNumber.Trim();

            AddressRegion? region;
            if (regionId == null)
            {
                // No country or region specified
                // Should start with +
                if (!phoneNumber.StartsWith('+'))
                {
                    return null;
                }

                region = All.FirstOrDefault(c => phoneNumber.StartsWith("+" + c.Idd));
            }
            else if (phoneNumber.StartsWith('+'))
            {
                // Specify country or region
                region = All.FirstOrDefault(c => phoneNumber.StartsWith("+" + c.Idd));
            }
            else
            {
                // Default country or region
                region = GetById(regionId);
            }

            if (region == null)
            {
                return null;
            }

            // Format the phone
            return region.FormatPhone(phoneNumber);
        }

        /// <summary>
        /// Create phones
        /// 创建多个电话对象
        /// </summary>
        /// <param name="phoneNumbers">Phone numbers</param>
        /// <param name="regionId">Default country or region id</param>
        /// <returns>Result</returns>
        public static IEnumerable<Phone> CreatePhones(IEnumerable<string> phoneNumbers, string? regionId = null)
        {
            foreach (var phoneNumber in phoneNumbers)
            {
                var phone = CreatePhone(phoneNumber, regionId);
                if (phone == null)
                    yield break;

                yield return phone;
            }
        }

        /// <summary>
        /// Phone number with country
        /// 带有国家标识的电话号码
        /// </summary>
        public record Phone
        {
            /// <summary>
            /// Phone number
            /// 电话号码
            /// </summary>
            public string PhoneNumber { get; init; }

            /// <summary>
            /// Is mobile
            /// 是否为移动号码
            /// </summary>
            public bool IsMobile { get; init; }

            /// <summary>
            /// Country or region
            /// 所在国家或地区
            /// </summary>
            public string Region { get; init; }

            internal Phone(string phoneNumber, bool isMobile, string region)
            {
                PhoneNumber = phoneNumber;
                IsMobile = isMobile;
                Region = region;
            }

            /// <summary>
            /// Convert to international dialing format
            /// 转换为国际拨号格式
            /// </summary>
            /// <param name="exitCode">Exit code, default is standard +</param>
            /// <returns>Result</returns>
            public string ToInternationalFormat(string exitCode = "+")
            {
                return string.Concat(exitCode, GetById(Region)?.Idd, PhoneNumber.TrimStart('0'));
            }
        }

        /// <summary>
        /// Format phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="isMobile">Is mobile</param>
        /// <returns>Result</returns>
        public Phone? FormatPhone(string phoneNumber, bool? isMobile = null)
        {
            if (IsValid(ref phoneNumber, ref isMobile))
            {
                return new Phone(phoneNumber, isMobile.GetValueOrDefault(), Id);
            }

            return null;
        }

        /// <summary>
        /// Is a valid phone number
        /// 是否是有效的电话号码
        /// </summary>
        /// <param name="phoneNumber">Phone number, ref as formated result</param>
        /// <param name="isMobile">Is mobile, null to be decided</param>
        /// <param name="country">Country</param>
        /// <returns>Result</returns>
        public bool IsValid(ref string phoneNumber, ref bool? isMobile)
        {
            // Remove empties
            phoneNumber = phoneNumber.Trim();

            if (phoneNumber.Length < 7)
            {
                // All phone numbers should be longer than 7 characters
                return false;
            }

            // Remove all other characters
            phoneNumber = string.Concat(phoneNumber.Where(c => c == '+' || c is (>= '0' and <= '9')));

            if (phoneNumber.StartsWith('+') && !phoneNumber.StartsWith('+' + Idd))
            {
                // Invalid IDD
                return false;
            }

            if (PhoneValidator != null)
            {
                // Do the validation
                return PhoneValidator(ref phoneNumber, ref isMobile, Idd);
            }

            return true;
        }
    }
}
