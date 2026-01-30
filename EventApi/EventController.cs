using Domain;
using Microsoft.AspNetCore.Mvc;

namespace EventApi;

[ApiController]
[Route("")]
public sealed class EventController(IEventService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var @event = await service.GetEventByIdAsync(id);
        return @event is not null ? Ok(@event) : NotFound();
    }

    [HttpGet("season/{season}")]
    public async Task<IActionResult> GetAllForSeason([FromRoute] int season)
    {
        var events = await service.GetAllEventsForSeasonAsync(season);
        return Ok(events);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] EventRecord @event)
    {
        await service.AddAsync(@event);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EventRecord @event)
    {
        var success = await service.UpdateAsync(@event);
        return success ? NoContent() : NotFound();
    }
}