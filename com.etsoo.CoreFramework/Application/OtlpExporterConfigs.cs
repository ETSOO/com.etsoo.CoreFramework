using OpenTelemetry.Exporter;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// OTLP exporter service options
    /// OTLP 导出服务选项
    /// </summary>
    public record OtlpExporterService
    {
        public OtlpExportProtocol? Protocol { get; set; }
        public Uri Endpoint { get; set; } = default!;
        public string? Headers { get; set; }
    }

    /// <summary>
    /// OTLP exporter options
    /// OTLP 导出选项
    /// </summary>
    public record OtlpExporterConfigs
    {
        public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.HttpProtobuf;

        public string? Headers { get; set; }

        public OtlpExporterService Logging { get; set; } = default!;

        public OtlpExporterService? Metrics { get; set; }

        public OtlpExporterService? Tracing { get; set; }
    }
}
