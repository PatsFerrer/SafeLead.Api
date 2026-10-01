using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SafeLead.Api.DTOs;

namespace SafeLead.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("PerIpRateLimit")]
public class LeadsController(ILogger<LeadsController> logger) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateLeadRequest request)
    {
        // Log estruturado (sem expor dados sensíveis desnecessários)
        logger.LogInformation("Novo lead recebido de {Email}", request.Email);

        return Ok(new
        {
            message = "Orçamento recebido com sucesso! Entraremos em contato em breve."
        });
    }
}