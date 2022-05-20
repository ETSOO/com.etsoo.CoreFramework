namespace com.etsoo.Address
{
    internal class AddressRegionPhoneComparer : IEqualityComparer<AddressRegion.Phone>
    {
        /// <summary>
        /// Equal check
        /// </summary>
        /// <param name="x">Object 1</param>
        /// <param name="y">Object 2</param>
        /// <returns>Result</returns>
        public bool Equals(AddressRegion.Phone? x, AddressRegion.Phone? y)
        {
            return x?.PhoneNumber == y?.PhoneNumber &&
                x?.IsMobile == y?.IsMobile &&
                x?.Region == y?.Region;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        public int GetHashCode(AddressRegion.Phone obj)
        {
            return obj.PhoneNumber.GetHashCode() ^ obj.IsMobile.GetHashCode() ^ obj.Region.GetHashCode();
        }
    }
}
