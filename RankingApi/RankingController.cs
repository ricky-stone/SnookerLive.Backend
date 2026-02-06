using Domain;
using Microsoft.AspNetCore.Mvc;

namespace SnookerLive;

[ApiController]
[Route("")]
public sealed class RankingController(IRankingService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var ranking = await service.GetRankingByIdAsync(id);
        return ranking is not null ? Ok(ranking) : NotFound();
    }

    [HttpGet]
    [Route("/type/{type}/{season:int}/{page:int?}/{pageSize:int?}")]
    public async Task<IActionResult> GetByTypeAndSeason([FromRoute] string type, [FromRoute] int season, [FromRoute] int page = 0, [FromRoute] int pageSize = 0)
    {
        var rankings = await service.GetRankingsByTypeAndSeasonAsync(type, season, page, pageSize);

        var result = new
        {
          Rankings = rankings,
          Next = (page > 0 && pageSize > 0 && rankings.Count == pageSize) ? $"{type}/{season}/{page + 1}/{pageSize}" : null  
        };

        return Ok(result);
    }

    [HttpGet]
    [Route("{type}/{season:int}/{page:int?}/{pageSize:int?}")]
    public async Task<IActionResult> GetByValueTypeAndSeason([FromRoute] string type, [FromRoute] int season, [FromRoute] int page = 0, [FromRoute] int pageSize = 0)
    {
        var rankings = await service.GetRankingsByValueTypeAndSeasonAsync(type, season, page, pageSize);

        var result = new
        {
          Rankings = rankings,
          Next = (page > 0 && pageSize > 0 && rankings.Count == pageSize) ? $"{type}/{season}/{page + 1}/{pageSize}" : null  
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] RankingRecord ranking)
    {
        await service.AddAsync(ranking);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] RankingRecord ranking)
    {
        var success = await service.UpdateAsync(ranking);
        return success ? NoContent() : NotFound();
    }
}