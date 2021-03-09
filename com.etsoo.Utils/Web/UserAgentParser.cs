using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UAParser;

namespace com.etsoo.Utils.Web
{
    /// <summary>
    /// User agent parser
    /// 用户代理解析器
    /// </summary>
    public class UserAgentParser
    {
        const string other = "Other";
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
        /// To Json data
        /// 获取Json数据
        /// </summary>
        /// <returns>Json string</returns>
        public async Task<string> ToJsonAsync(JsonSerializerOptions? options = null)
        {
            // Define a writer
            var writer = new ArrayBufferWriter<byte>();

            // Write
            await ToJsonAsync(writer, options);

            // Return
            return Encoding.UTF8.GetString(writer.WrittenSpan);
        }

        private void WriteIntData(Utf8JsonWriter writer, string value)
        {
            var intData = StringUtil.TryParse<int>(value);
            if (intData.HasValue)
                writer.WriteNumberValue(intData.Value);
            else
                writer.WriteStringValue(value);
        }

        /// <summary>
        /// To Json data
        /// 获取Json数据
        /// </summary>
        /// <returns>Json string</returns>
        public async Task ToJsonAsync(IBufferWriter<byte> writer, JsonSerializerOptions? options = null)
        {
            // Default options
            options ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);

            // Utf8JsonWriter
            using var w = new Utf8JsonWriter(writer, new JsonWriterOptions { Encoder = options.Encoder, Indented = options.WriteIndented });

            // Object start {
            w.WriteStartObject();

            if (info != null)
            {
                // Device
                w.WriteStartObject(options.ConvertName("Device"));

                // Current value
                w.WriteString(options.ConvertName("Type"), Device.ToString());
                w.WriteBoolean(options.ConvertName("IsSpider"), IsSpider);
                w.WriteBoolean(options.ConvertName("IsMobile"), IsMobile);

                if (info.Device.Family != other && !IsSpider)
                {
                    w.WriteString(options.ConvertName("Family"), info.Device.Family);

                    if (!string.IsNullOrEmpty(info.Device.Brand))
                    {
                        w.WriteString(options.ConvertName("Brand"), info.Device.Brand);
                    }

                    if (!string.IsNullOrEmpty(info.Device.Model))
                    {
                        w.WriteString(options.ConvertName("Model"), info.Device.Model);
                    }
                }

                w.WriteEndObject();

                // OS
                if (info.OS.Family != other && !IsSpider)
                {
                    w.WriteStartObject(options.ConvertName("OS"));

                    w.WriteString(options.ConvertName("Family"), info.OS.Family);

                    if (!string.IsNullOrEmpty(info.OS.Major))
                    {
                        w.WritePropertyName(options.ConvertName("Major"));
                        WriteIntData(w, info.OS.Major);
                    }

                    if (!string.IsNullOrEmpty(info.OS.Minor))
                    {
                        w.WritePropertyName(options.ConvertName("Minor"));
                        WriteIntData(w, info.OS.Minor);
                    }

                    if (!string.IsNullOrEmpty(info.OS.Patch))
                    {
                        w.WritePropertyName(options.ConvertName("Patch"));
                        WriteIntData(w, info.OS.Patch);
                    }

                    w.WriteEndObject();
                }

                // UA
                if (info.UA.Family != other)
                {
                    w.WriteStartObject(options.ConvertName("UA"));

                    w.WriteString(options.ConvertName("Family"), info.UA.Family);

                    if (!string.IsNullOrEmpty(info.UA.Major))
                    {
                        w.WritePropertyName(options.ConvertName("Major"));
                        WriteIntData(w, info.UA.Major);
                    }

                    if (!string.IsNullOrEmpty(info.UA.Minor))
                    {
                        w.WritePropertyName(options.ConvertName("Minor"));
                        WriteIntData(w, info.UA.Minor);
                    }

                    if (!string.IsNullOrEmpty(info.UA.Patch))
                    {
                        w.WritePropertyName(options.ConvertName("Patch"));
                        WriteIntData(w, info.UA.Patch);
                    }

                    w.WriteEndObject();
                }
            }

            // Object end }
            w.WriteEndObject();

            // Flush & dispose
            await w.DisposeAsync();
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
