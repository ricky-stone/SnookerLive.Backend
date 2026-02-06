using Domain;
using Microsoft.AspNetCore.Mvc;

namespace SnookerLive;

[ApiController]
[Route("")]
public sealed class RoundController(IRoundService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var round = await service.GetRoundByIdAsync(id);
        return round is not null ? Ok(round) : NotFound();
    }

    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetByEventId([FromRoute] int eventId)
    {
        var rounds = await service.GetRoundsByEventIdAsync(eventId);
        return rounds.Count > 0 ? Ok(rounds) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RoundRecord round)
    {
        await service.AddAsync(round);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RoundRecord round)
    {
        var success = await service.UpdateAsync(round);
        return success ? NoContent() : NotFound();
    }
}