using Microsoft.AspNetCore.Mvc;

namespace SnookerLive;

[ApiController]
[Route("events")]
public sealed class EventController(
    IEventService eventService) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var @event = await eventService.GetEventByIdAsync(id);
        return @event is not null ? Ok(@event) : NotFound();
    }
}