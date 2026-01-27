using Domain;
using Microsoft.AspNetCore.Mvc;

namespace MatchApi;

[ApiController]
[Route("")]
public sealed class MatchController(IMatchService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var match = await service.GetMatchByIdAsync(id);
        return match is not null ? Ok(match) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] MatchRecord match)
    {
        await service.AddAsync(match);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] MatchRecord match)
    {
        var success = await service.UpdateAsync(match);
        return success ? NoContent() : NotFound();
    }
}