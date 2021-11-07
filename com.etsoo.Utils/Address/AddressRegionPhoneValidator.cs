namespace com.etsoo.Utils.Address
{
    /// <summary>
    /// Country or region phone validator delegate
    /// </summary>
    /// <param name="phoneNumber">Phone numbers</param>
    /// <param name="isMobile">Is mobile</param>
    /// <param name="Idd">IDD</param>
    /// <returns>Result</returns>
    public delegate bool AddressRegionPhoneValidatorDel(ref string phoneNumber, ref bool? isMobile, string Idd);

    /// <summary>
    /// Country or region phone validator
    /// </summary>
    public static class AddressRegionPhoneValidator
    {
        /// <summary>
        /// CN
        /// 中国
        /// </summary>
        public static AddressRegionPhoneValidatorDel CN => (ref string phoneNumber, ref bool? isMobile, string Idd) =>
        {
            // International format
            var intl = phoneNumber.StartsWith('+');

            if (intl)
            {
                // Remove IDD
                phoneNumber = phoneNumber[(Idd.Length + 1)..];
            }

            // https://zh.wikipedia.org/wiki/%E4%B8%AD%E5%9B%BD%E5%A4%A7%E9%99%86%E7%A7%BB%E5%8A%A8%E7%BB%88%E7%AB%AF%E9%80%9A%E4%BF%A1%E5%8F%B7%E7%A0%81
            var isMobileActual = phoneNumber.StartsWith("13") || phoneNumber.StartsWith("15") 
                || phoneNumber.StartsWith("16") || phoneNumber.StartsWith("17") 
                || phoneNumber.StartsWith("18") || phoneNumber.StartsWith("19");

            if (isMobile == null)
            {
                isMobile = isMobileActual;
            }
            else if (isMobile != isMobileActual)
            {
                // Required is not the same with actual
                return false;
            }

            if (intl && !isMobile.GetValueOrDefault())
            {
                // Add zero
                phoneNumber = '0' + phoneNumber;
            }

            if (isMobileActual)
            {
                return phoneNumber.Length == 11;
            }
            else
            {
                if (!phoneNumber.StartsWith('0'))
                    return false;

                return phoneNumber.Length is >= 11 and <= 12;
            }
        };

        /// <summary>
        /// NZ
        /// 新西兰
        /// </summary>
        public static AddressRegionPhoneValidatorDel NZ => (ref string phoneNumber, ref bool? isMobile, string Idd) =>
        {
            // https://www.tnzi.com/numbering-plan
            // International format
            var intl = phoneNumber.StartsWith('+');

            if (intl)
            {
                // Remove IDD
                phoneNumber = phoneNumber[(Idd.Length + 1)..];

                // Add zero
                phoneNumber = '0' + phoneNumber;
            }

            var isMobileActual = phoneNumber.StartsWith("02") && !phoneNumber.StartsWith("0240");

            if (isMobile == null)
            {
                isMobile = isMobileActual;
            }
            else if (isMobile != isMobileActual)
            {
                // Required is not the same with actual
                return false;
            }

            return phoneNumber.Length is >= 9 and <= 11;
        };
    }
}
