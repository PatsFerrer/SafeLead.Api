using System.ComponentModel.DataAnnotations;

namespace SafeLead.Api.Configurations
{
    public class SecuritySettings
    {
        public const string SectionName = "SecuritySettings";

        [Required(ErrorMessage = "AllowedOrigins é obrigatório na configuração.")]
        [MinLength(1, ErrorMessage = " informe pelo menos uma origem no AllowedOrigins.")]
        public string[] AllowedOrigins { get; init; } = [];

        [Range(1, 1000, ErrorMessage = "RateLimitPermitLimit deve estar entre 1 e 1000.")]
        public int RateLimitPermitLimit { get; init; }

        [Range(1, 3600, ErrorMessage = "RateLimitWindowSeconds deve estar entre 1 e 3600.")]
        public int RateLimitWindowSeconds { get; init; }
    }
}
