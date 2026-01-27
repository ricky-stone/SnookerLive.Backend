using Domain;
using Microsoft.AspNetCore.Mvc;

namespace SessionApi;

[ApiController]
[Route("")]
public sealed class SessionController(ISessionService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var session = await service.GetSessionByIdAsync(id);
        return session is not null ? Ok(session) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] SessionRecord session)
    {
        await service.AddAsync(session);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SessionRecord session)
    {
        var success = await service.UpdateAsync(session);
        return success ? NoContent() : NotFound();
    }
}