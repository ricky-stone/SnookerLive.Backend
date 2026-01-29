using Domain;
using Microsoft.AspNetCore.Mvc;

namespace SessionApi;

[ApiController]
[Route("")]
public sealed class SessionController(ISessionService service) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var session = await service.GetSessionByIdAsync(id);
        return session is not null ? Ok(session) : NotFound();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Upsert([FromRoute] string id, [FromBody] SessionRecord session)
    {
        if (string.IsNullOrWhiteSpace(session.Id) || session.Id != id)
            return BadRequest("Body id must match route id.");

        await service.UpsertAsync(session);
        return NoContent();
    }
}