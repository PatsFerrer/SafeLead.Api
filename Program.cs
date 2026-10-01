using SafeLead.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Limita o tamanho máximo do corpo da requisição em 10 KB (proteção contra payloads gigantes)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSecurityServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Ativa os nossos middlewares de segurança (Helmet, CORS e Rate Limit)
app.UseSecurityMiddlewares();

app.UseAuthorization();
app.MapControllers();

app.Run();