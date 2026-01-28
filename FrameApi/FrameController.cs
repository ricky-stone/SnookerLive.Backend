using Domain;
using Microsoft.AspNetCore.Mvc;

namespace FrameApi;

[ApiController]
[Route("")]
public sealed class FrameController(IFrameService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var frame = await service.GetFrameByIdAsync(id);
        return frame is not null ? Ok(frame) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FrameRecord frame)
    {
        await service.AddAsync(frame);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FrameRecord frame)
    {
        var success = await service.UpdateAsync(frame);
        return success ? NoContent() : NotFound();
    }
}