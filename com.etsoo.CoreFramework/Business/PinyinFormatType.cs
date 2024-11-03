namespace com.etsoo.CoreFramework.Business
{
    /// <summary>
    /// Pinyin format type
    /// 拼音格式类型
    /// </summary>
    public enum PinyinFormatType : byte
    {
        /// <summary>
        /// Full Pinyin without tone
        /// 不带声调的全拼
        /// </summary>
        Full,

        /// <summary>
        /// Initial letter
        /// 首字母
        /// </summary>
        Initial,

        /// <summary>
        /// Full Pinyin with tone
        /// 带声调的全拼
        /// </summary>
        Tone
    }
}
