using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Pinyin format type
    /// 拼音格式类型
    /// </summary>
    public enum PinyinFormatType : byte
    {
        /// <summary>
        /// Default, full Pinyin without tone
        /// 默认，不带声调的全拼
        /// </summary>
        Default,

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

    /// <summary>
    /// Get Pinyin request data
    /// 获取拼音请求数据
    /// </summary>
    public record PinyinRQ
    {
        /// <summary>
        /// Input string
        /// 输入的字符串
        /// </summary>
        [StringLength(512)]
        public required string Input { get; init; }

        /// <summary>
        /// Return format
        /// 返回格式
        /// </summary>
        public PinyinFormatType Format { get; init; }

        /// <summary>
        /// Is name
        /// 是否为姓名
        /// </summary>
        public bool? IsName { get; init; }
    }
}
