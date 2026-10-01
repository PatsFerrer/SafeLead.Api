using System.Threading.RateLimiting;

namespace SafeLead.Api.Configurations;

public static class SecurityConfiguration
{
    public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Valida se o secrets.json / variáveis de ambiente estão preenchidos corretamente ao ligar a API
        services.AddOptions<SecuritySettings>()
            .Bind(configuration.GetSection(SecuritySettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var securitySettings = configuration
            .GetSection(SecuritySettings.SectionName)
            .Get<SecuritySettings>() ?? new SecuritySettings();

        // 2. CORS Restrito: aceita apenas a URL do seu Front-end e apenas métodos necessários
        services.AddCors(options =>
        {
            options.AddPolicy("StrictCorsPolicy", policy =>
            {
                policy.WithOrigins(securitySettings.AllowedOrigins)
                      .WithMethods("GET", "POST")
                      .WithHeaders("Content-Type", "Accept");
            });
        });

        // 3. Rate Limiting por IP: bloqueia flood de formulário e retorna erro 429 (Too Many Requests)
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("PerIpRateLimit", httpContext =>
            {
                // Pega o IP real (mesmo quando passar pela Cloudflare no deploy)
                var clientIp = httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
                               ?? httpContext.Connection.RemoteIpAddress?.ToString()
                               ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = securitySettings.RateLimitPermitLimit,
                    Window = TimeSpan.FromSeconds(securitySettings.RateLimitWindowSeconds),
                    QueueLimit = 0
                });
            });
        });

        return services;
    }

    public static WebApplication UseSecurityMiddlewares(this WebApplication app)
    {
        // 4. Equivalente ao Helmet: adiciona cabeçalhos HTTP de proteção em todas as respostas
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "0");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'");
            await next();
        });

        app.UseCors("StrictCorsPolicy");
        app.UseRateLimiter();

        return app;
    }
}