namespace com.etsoo.Localization.Chinese
{
    /// <summary>
    /// Character PinYin
    /// 汉字拼音
    /// </summary>
    public record CharPinyin
    {
        /// <summary>
        /// PinYin
        /// 拼音
        /// </summary>
        public ReadOnlyMemory<char> Py { get; }

        /// <summary>
        /// Tone
        /// 声调
        /// </summary>
        public CharTone Tone { get; }

        /// <summary>
        /// Is family name
        /// 是否为姓氏
        /// </summary>
        public bool? IsFamilyName { get; }

        /// <summary>
        /// Special cases
        /// 特殊情况
        /// </summary>
        public string[]? Cases { get; }

        public CharPinyin(ReadOnlySpan<char> py, CharTone tone, bool? isFamilyName = null, string[]? cases = null)
        {
            Py = py.ToArray();
            Tone = tone;
            IsFamilyName = isFamilyName;
            Cases = cases;
        }
    }
}
