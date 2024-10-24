namespace com.etsoo.Localization.Chinese
{
    /// <summary>
    /// Chinese character
    /// 中文字符
    /// </summary>
    public record ChineseCharacter
    {
        /// <summary>
        /// PinYins
        /// 拼音
        /// </summary>
        public required CharPinyin[] PinYins { get; init; }
    }
}
