using Microsoft.AspNetCore.Mvc;                      // Imports the ASP.NET Core MVC namespace for building API controllers
using ZeroTrustAPI.Api.Services.Interfaces;          // Imports the interface for the health check service

namespace ZeroTrustAPI.Api.Controllers;              // Defines the namespace where this controller lives

[ApiController]                                      // Enables automatic model validation, binding, and other API-specific behaviors
[Route("health")]                                    // Maps HTTP requests to this controller using the literal route "health"
public class HealthController : ControllerBase       // Defines the controller class that inherits from ControllerBase (no view support)
{
    private readonly IHealthService _healthService;  // Declares a private read-only field for the health service dependency

    public HealthController(IHealthService healthService) => _healthService = healthService;  // Constructor: receives IHealthService via dependency injection and assigns it to the field

    [HttpGet]                                        // Indicates that the following method handles HTTP GET requests
    public async Task<IActionResult> Get()           // Method is async, returns IActionResult (HTTP response)
    {
        var result = await _healthService.GetHealthAsync();  // Calls the async health service method and awaits the result

        return result.IsHealthy ? Ok(result) : StatusCode(503, result);  // If healthy: returns 200 OK with result; else: returns 503 Service Unavailable with result
    }
}