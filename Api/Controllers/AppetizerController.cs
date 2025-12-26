using Microsoft.AspNetCore.Mvc;

namespace MenuSoda.Controllers;

[ApiController]
[Route("[controller]")]
public class AppetizerController : ControllerBase
{
   private readonly ILogger<AppetizerController> _logger;

    public AppetizerController(ILogger<AppetizerController> logger)
    {
        _logger = logger;
    }

    
}