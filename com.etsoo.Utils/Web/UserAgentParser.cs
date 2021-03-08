using System.Collections.Generic;
using UAParser;

namespace com.etsoo.Utils.Web
{
    /// <summary>
    /// User agent parser
    /// 用户代理解析器
    /// </summary>
    public class UserAgentParser
    {
        private readonly ClientInfo? info;

        /// <summary>
        /// Client info
        /// 客户端信息
        /// </summary>
        public ClientInfo? Client => info;

        /// <summary>
        /// Is user agent not null and empty and could be parsed
        /// 用户代理是否不为null和空并且可以解析
        /// </summary>
        public bool Valid { get; }

        /// <summary>
        /// Is spider
        /// 是否为爬虫
        /// </summary>
        public bool IsSpider { get; }

        /// <summary>
        /// Is mobile
        /// 是否为手机
        /// </summary>
        public bool IsMobile { get; }

        /// <summary>
        /// Get device type
        /// 获取设备类型
        /// </summary>
        public UserAgentDevice Device { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="userAgent">User agent</param>
        public UserAgentParser(string? userAgent)
        {
            // Ignore null and empty content
            if (string.IsNullOrEmpty(userAgent))
                return;

            // Parse
            var parser = Parser.GetDefault(new ParserOptions { UseCompiledRegex = true });

            // Client info
            info = parser.Parse(userAgent);
            if (info == null)
                return;

            // Valid
            Valid = true;

            // Is spider
            IsSpider = info.Device.IsSpider;

            // Device
            if (!IsSpider)
            {
                Device = GetDevice(info);
                IsMobile = Device == UserAgentDevice.Mobile;
            }
        }

        /// <summary>
        /// To readable string
        /// 获取可读的字符串
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            if (info == null || !Valid)
                return string.Empty;

            var other = "Other";

            var parts = new List<string>();
            if (info.Device.Family != other)
                parts.Add(info.Device.ToString());
            if (info.OS.Family != other)
                parts.Add(info.OS.ToString());
            if (info.UA.Family != other)
                parts.Add(info.UA.ToString());

            return string.Join(", ", parts);
        }

        private UserAgentDevice GetDevice(ClientInfo c)
        {
            switch (c.OS.Family)
            {
                case "Android":
                    {
                        if (c.Device.Family == "Generic Tablet" || c.Device.Family.Contains("Lenovo"))
                            return UserAgentDevice.Tablet;

                        return UserAgentDevice.Mobile;
                    }
                case "iOS":
                    {
                        if (c.Device.Family == "iPad")
                            return UserAgentDevice.Tablet;

                        return UserAgentDevice.Mobile;
                    }
            }

            return UserAgentDevice.Desktop;
        }
    }
}
