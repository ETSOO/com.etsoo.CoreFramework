using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Jwt authentication service settings
    /// Jwt认证服务配置
    /// </summary>
    public record JwtSettings
    {
        public string? DefaultIssuer { get; set; }
        public string? DefaultAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public IEnumerable<string>? ValidIssuers { get; set; }
        public string? ValidAudience { get; set; }
        public IEnumerable<string>? ValidAudiences { get; set; }
        public IEnumerable<string>? TokenUrls { get; set; }
        public bool? ValidateAudience { get; set; }
        public string? SecurityAlgorithms { get; set; }
        public int? AccessTokenMinutes { get; set; }
        public int? RefreshTokenDays { get; set; }

        [Required]
        public string EncryptionKey { get; set; } = string.Empty;

        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }
    }

    [OptionsValidator]
    public partial class ValidateJwtSettings : IValidateOptions<JwtSettings>
    {
    }
}
