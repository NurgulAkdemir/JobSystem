using Microsoft.AspNetCore.Mvc;

namespace JobSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }
    [HttpGet("details")]
    public IActionResult GetDetails()
    {
        return Ok(new
        {
            status = "Healthy",
            machine = Environment.MachineName,
            os = Environment.OSVersion.ToString(),
            processors = Environment.ProcessorCount,
            memory = GC.GetTotalMemory(false)
        });
    }
}