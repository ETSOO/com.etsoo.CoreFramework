namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Jwt authentication service settings
    /// Jwt认证服务配置
    /// </summary>
    public record JwtSettings
    {
        public string? DefaultIssuer { get; init; }
        public string? DefaultAudience { get; init; }
        public string? ValidIssuer { get; init; }
        public IEnumerable<string>? ValidIssuers { get; init; }
        public string? ValidAudience { get; init; }
        public IEnumerable<string>? ValidAudiences { get; init; }
        public IEnumerable<string>? TokenUrls { get; init; }
        public bool? ValidateAudience { get; init; }
        public string? SecurityAlgorithms { get; init; }
        public int? AccessTokenMinutes { get; init; }
        public int? RefreshTokenDays { get; init; }
        public string EncryptionKey { get; init; } = string.Empty;
        public string? PublicKey { get; init; }
        public string? PrivateKey { get; init; }
    }
}
