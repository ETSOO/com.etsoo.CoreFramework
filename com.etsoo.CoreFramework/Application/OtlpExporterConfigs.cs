using OpenTelemetry.Exporter;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// OTLP exporter options
    /// OTLP 导出选项
    /// </summary>
    public record OtlpExporterConfigs
    {
        public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.HttpProtobuf;
        public Uri Endpoint { get; set; } = default!;
        public string? Headers { get; set; }
    }
}
