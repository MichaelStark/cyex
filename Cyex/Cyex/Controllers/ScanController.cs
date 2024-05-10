using Cyex.Interfaces;
using Cyex.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cyex.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ScanController(IScanService scanService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Scan([FromBody] ScanRequest request)
    {
        // TODO handle exceptions
        var result = await scanService.Scan(request);
        return Ok(result);
    }
}