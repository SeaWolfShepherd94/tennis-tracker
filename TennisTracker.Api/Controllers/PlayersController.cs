using Microsoft.AspNetCore.Mvc;
using TennisTracker.Api.Interfaces;
using TennisTracker.Api.Models;

namespace TennisTracker.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]")]
public sealed class PlayersController(IPlayerService playerService) : ControllerBase
{
    private readonly IPlayerService _playerService = playerService
        ?? throw new ArgumentNullException(nameof(playerService));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlayerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<PlayerDto> players = await _playerService.GetAllAsync().ConfigureAwait(false);
        return Ok(players);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        PlayerDto? player = await _playerService.GetByIdAsync(id).ConfigureAwait(false);
        if (player is null)
            return NotFound(new { error = $"Player {id} not found." });
        return Ok(player);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PlayerDto dto)
    {
        PlayerDto created = await _playerService.CreateAsync(dto).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PlayerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PlayerDto dto)
    {
        PlayerDto? updated = await _playerService.UpdateAsync(id, dto).ConfigureAwait(false);
        if (updated is null)
            return NotFound(new { error = $"Player {id} not found." });
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        bool deleted = await _playerService.DeleteAsync(id).ConfigureAwait(false);
        if (!deleted)
            return NotFound(new { error = $"Player {id} not found." });
        return NoContent();
    }
}
