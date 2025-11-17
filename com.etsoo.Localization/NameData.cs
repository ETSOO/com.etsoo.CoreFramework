namespace com.etsoo.Localization
{
    /// <summary>
    /// Name data
    /// 姓名数据
    /// </summary>
    public record NameData
    {
        /// <summary>
        /// Pinyin initials
        /// 拼音首字母
        /// </summary>
        public string? PinyinInitials { get; init; }

        /// <summary>
        /// Family name
        /// 姓氏
        /// </summary>
        public string? FamilyName { get; init; }

        /// <summary>
        /// Given name
        /// 名
        /// </summary>
        public string? GivenName { get; init; }

        /// <summary>
        /// Latin family name
        /// 拉丁姓（拼音）
        /// </summary>
        public string? LatinFamilyName { get; set; }

        /// <summary>
        /// Latin given name
        /// 拉丁名（拼音）
        /// </summary>
        public string? LatinGivenName { get; set; }
    }
}
