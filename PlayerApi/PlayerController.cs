using Domain;
using Microsoft.AspNetCore.Mvc;

namespace SnookerLive;

[ApiController]
[Route("")]
public sealed class PlayerController(IPlayerService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var player = await service.GetPlayerByIdAsync(id);
        return player is not null ? Ok(player) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] PlayerRecord player)
    {
        await service.AddAsync(player);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PlayerRecord player)
    {
        var success = await service.UpdateAsync(player);
        return success ? NoContent() : NotFound();
    }
}