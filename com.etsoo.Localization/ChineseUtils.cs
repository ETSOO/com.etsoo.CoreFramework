using com.etsoo.Localization.Chinese;
using System.Text.RegularExpressions;

namespace com.etsoo.Localization
{
    /// <summary>
    /// Chinese utilities
    /// 中文工具
    /// </summary>
    public static partial class ChineseUtils
    {
        private static ReadOnlySpan<char> FirstLetterUpper(this ReadOnlyMemory<char> chars)
        {
            var span = chars.ToArray();
            if (span.Length > 0)
            {
                span[0] = char.ToUpper(span[0]);
            }

            return span;
        }

        /// <summary>
        /// To pinyin initials
        /// 生成拼音首字母，比如：我爱重庆 -> WACQ
        /// </summary>
        /// <param name="pinyins"></param>
        /// <returns></returns>
        public static string ToInitials(this IEnumerable<CharPinyin> pinyins)
        {
            return new string(pinyins.Select(py => py.Py.Length > 0 ? char.ToUpper(py.Py.ToArray()[0]) : '\0').ToArray());
        }

        /// <summary>
        /// To pinyin string
        /// 生成拼音字符串，比如：我爱重庆 -> Wo Ai Chong Qing 或 Wo3 Ai4 Chong2 Qing1
        /// </summary>
        /// <param name="pinyins">Pinyin collections</param>
        /// <param name="withTone">With tone or not</param>
        /// <returns>Result</returns>
        public static string ToPinyin(this IEnumerable<CharPinyin> pinyins, bool withTone = false)
        {
            return string.Join(" ", pinyins.Select(py => $"{py.Py.FirstLetterUpper()}{(withTone ? ((byte)py.Tone).ToString() : string.Empty)}"));
        }

        /// <summary>
        /// Get Chinese character
        /// 获取中文字符
        /// </summary>
        /// <param name="input">Input character</param>
        /// <returns>Result</returns>
        public static ChineseCharacter? GetCharacter(char input)
        {
            if (ChineseCharacters.Characters1.TryGetValue(input, out var c1))
            {
                return c1;
            }
            else if (ChineseCharacters.Characters2.TryGetValue(input, out var c2))
            {
                return c2;
            }
            else if (ChineseCharacters.Characters3.TryGetValue(input, out var c3))
            {
                return c3;
            }
            else if (ChineseCharacters.Characters4.TryGetValue(input, out var c4))
            {
                return c4;
            }
            else if (ChineseCharacters.Characters5.TryGetValue(input, out var c5))
            {
                return c5;
            }
            else if (ChineseCharacters.Characters6.TryGetValue(input, out var c6))
            {
                return c6;
            }
            else if (ChineseCharacters.Characters7.TryGetValue(input, out var c7))
            {
                return c7;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get pinyin for the input
        /// 获取输入的拼音
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isName">Is a person's name</param>
        /// <returns>Result</returns>
        public static IEnumerable<CharPinyin> GetPinyin(string input, bool isName = false)
        {
            return input.Select((c, index) =>
            {
                if (!c.IsChinese())
                {
                    return null;
                }

                var cc = GetCharacter(c);
                if (cc == null)
                {
                    // Not all Chinese characters are in the dictionary
                    return null;
                }

                var plen = cc.PinYins.Length;
                if (plen == 0)
                {
                    // No pinyin is defined
                    return null;
                }

                if (plen > 1)
                {
                    // Multiple pinyin

                    // Check if it is a family name
                    if (isName && index == 0)
                    {
                        // First character is the family name
                        var items = cc.PinYins.Where(py => py.IsFamilyName == true);
                        if (items.Count() == 1)
                        {
                            // Only one family name
                            return items.First();
                        }
                    }

                    // Math the cases, two characters minimum
                    foreach (var py in cc.PinYins)
                    {
                        var cases = py.Cases;
                        if (cases == null)
                        {
                            continue;
                        }

                        foreach (var cs in cases)
                        {
                            // Like 我爱重庆
                            // c = 重, index = 3
                            // cs = 重庆, len = 2
                            var len = cs.Length;

                            // pos = 0
                            var pos = cs.IndexOf(c);
                            if (pos == -1 || index < pos)
                            {
                                continue;
                            }

                            var start = index - pos;
                            if (start + len > input.Length)
                            {
                                continue;
                            }

                            var pc = input.Substring(index - pos, len);
                            if (cs == pc)
                            {
                                return py;
                            }
                        }
                    }
                }

                // Default
                return cc.PinYins[0];
            }).Where(py => py != null).Select(py => py!);
        }

        /// <summary>
        /// Is Chinese character
        /// 是否为中文字符
        /// </summary>
        /// <param name="c">Character</param>
        /// <returns>Result</returns>
        public static bool IsChinese(this char c)
        {
            return ChineseRegex().IsMatch(c.ToString());
        }

        [GeneratedRegex(@"\p{IsCJKUnifiedIdeographs}")]
        private static partial Regex ChineseRegex();
    }
}
