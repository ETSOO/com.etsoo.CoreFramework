namespace com.etsoo.Address
{
    /// <summary>
    /// Address extensions
    /// 地址功能扩展
    /// </summary>
    public static class AddressExtensions
    {
        /// <summary>
        /// Unique phones
        /// 唯一的电话号码
        /// </summary>
        /// <param name="phones">Phones</param>
        /// <returns>Result</returns>
        public static IEnumerable<AddressRegion.Phone> UniquePhones(this IEnumerable<AddressRegion.Phone> phones)
        {
            return phones.Distinct(new AddressRegionPhoneComparer());
        }
    }
}
