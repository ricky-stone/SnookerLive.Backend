using Domain;
using Microsoft.AspNetCore.Mvc;

namespace SnookerLive;

[ApiController]
[Route("")]
public sealed class WatchOnController(IWatchOnService service) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var watchOn = await service.GetWatchOnByIdAsync(id);
        return watchOn is not null ? Ok(watchOn) : NotFound();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Upsert([FromRoute] string id, [FromBody] WatchOn watchOn)
    {
        if (string.IsNullOrWhiteSpace(watchOn.Id) || watchOn.Id != id)
            return BadRequest("Body id must match route id.");

        await service.UpsertAsync(watchOn);
        return NoContent();
    }
}