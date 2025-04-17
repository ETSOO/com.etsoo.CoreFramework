using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace com.etsoo.Address.Validators
{
    /// <summary>
    /// China PIN validator
    /// 中国身份证验证器
    /// </summary>
    public class ChinaPinValidator
    {
        /// <summary>
        /// Valid form or not
        /// 是否形式有效
        /// </summary>
        [MemberNotNullWhen(true, nameof(StateNum), nameof(CityNum), nameof(DistrictNum), nameof(Birthday), nameof(IsFemale))]
        public bool Valid { get; }

        /// <summary>
        /// State num id, like Hunan is 43
        /// 州省数字编号，比如湖南是 43
        /// </summary>
        public string? StateNum { get; }

        /// <summary>
        /// City num id, like Changsha is 4301
        /// 城市数字编号，比如长沙是 4301
        /// </summary>
        public string? CityNum { get; }

        /// <summary>
        /// District number id, like Ningxiang is 430124
        /// 区县数字编号，比如宁乡是 430124
        /// </summary>
        public string? DistrictNum { get; }

        /// <summary>
        /// Birthday
        /// 出生日期
        /// </summary>
        public DateTimeOffset? Birthday { get; }

        /// <summary>
        /// Is female
        /// 是否为女性
        /// </summary>
        public bool? IsFemale { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="pin">PIN</param>
        public ChinaPinValidator(string pin)
        {
            var len = pin.Length;
            if (len < 6) return;

            var state = pin[..2];
            if (int.TryParse(state, out var _))
            {
                StateNum = state;
            }

            var city = pin[..4];
            if (int.TryParse(city, out var _))
            {
                CityNum = city;
            }

            var district = pin[..6];
            if (int.TryParse(district, out var _))
            {
                DistrictNum = district;
            }

            if (len >= 14 && DateTimeOffset.TryParseExact(pin[6..14] + "+08", "yyyyMMddzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                Birthday = date;
            }

            if (len >= 17 && int.TryParse(pin.AsSpan(14, 3), out var g))
            {
                IsFemale = g % 2 == 0;
            }

            if (StateNum == null || CityNum == null || DistrictNum == null || Birthday == null || IsFemale == null || len != 18) return;

            var coefficients = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            var sum = pin[..17].Select((c, index) => (c - '0') * coefficients[index]).Sum() % 11;
            var checksum = sum switch
            {
                0 => '1',
                1 => '0',
                2 => 'X',
                _ => (12 - sum).ToString()[0]
            };

            var last = char.ToUpper(pin[^1]);
            Valid = checksum.Equals(last);
        }
    }
}
