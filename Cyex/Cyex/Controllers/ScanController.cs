using Cyex.Interfaces;
using Cyex.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cyex.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ScanController(IScanService scanService, ILogger<ScanController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Scan([FromBody] ScanRequest request)
    {
        try
        {
            var result = await scanService.Scan(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError("Something went wrong while Scan: {Message}", ex.Message);
            return BadRequest();
        }
    }
}