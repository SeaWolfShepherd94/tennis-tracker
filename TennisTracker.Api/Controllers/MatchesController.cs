using Microsoft.AspNetCore.Mvc;
using TennisTracker.Api.Interfaces;
using TennisTracker.Api.Models;

namespace TennisTracker.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
public sealed class MatchesController(IMatchService matchService) : ControllerBase
{
    private readonly IMatchService _matchService = matchService
        ?? throw new ArgumentNullException(nameof(matchService));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<MatchDto> matches = await _matchService.GetAllAsync().ConfigureAwait(false);
        return Ok(matches);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MatchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        MatchDto? match = await _matchService.GetByIdAsync(id).ConfigureAwait(false);
        if (match is null)
            return NotFound(new { error = $"Match {id} not found." });
        return Ok(match);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MatchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] MatchDto dto)
    {
        MatchDto created = await _matchService.CreateAsync(dto).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MatchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] MatchDto dto)
    {
        MatchDto? updated = await _matchService.UpdateAsync(id, dto).ConfigureAwait(false);
        if (updated is null)
            return NotFound(new { error = $"Match {id} not found." });
        return Ok(updated);
    }

    [HttpPut("{id:guid}/score")]
    [ProducesResponseType(typeof(MatchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveScore(Guid id, [FromBody] ScoreRequest request)
    {
        MatchDto? result = await _matchService.SaveScoreAsync(id, request).ConfigureAwait(false);
        if (result is null)
            return NotFound(new { error = $"Match {id} not found." });
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        bool deleted = await _matchService.DeleteAsync(id).ConfigureAwait(false);
        if (!deleted)
            return NotFound(new { error = $"Match {id} not found." });
        return NoContent();
    }

    [HttpPost("import-wimbledon")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportWimbledon()
    {
        int matchesCreated = await _matchService.ImportWimbledonDrawAsync().ConfigureAwait(false);
        return Ok(new { matchesCreated });
    }
}
