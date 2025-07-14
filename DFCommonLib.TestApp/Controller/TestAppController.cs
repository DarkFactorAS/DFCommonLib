

using DFCommonLib.Logger;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TestAppController : ControllerBase
{
    public TestAppController()
    {
        // Constructor logic if needed
        DFLogger.LogOutput(DFLogLevel.INFO, "TestAppController", "Controller initialized.");
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("TestApp is running!");
    }

    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return CreatedAtAction(nameof(Get), new { id = 1 }, value);
    }
}