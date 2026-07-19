using TennisTracker.Api.Interfaces;
using TennisTracker.Api.Models;
using TennisTracker.Core.Models;
using TennisTracker.Data.Interfaces;

namespace TennisTracker.Api.Services;

public sealed class PlayerService(IPlayerRepository playerRepository) : IPlayerService
{
    private readonly IPlayerRepository _playerRepository = playerRepository
        ?? throw new ArgumentNullException(nameof(playerRepository));

    public async Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        IEnumerable<Player> players = await _playerRepository.GetAllAsync().ConfigureAwait(false);
        return players.Select(ToDto);
    }

    public async Task<PlayerDto?> GetByIdAsync(Guid id)
    {
        Player? player = await _playerRepository.GetByIdAsync(id).ConfigureAwait(false);
        return player is null ? null : ToDto(player);
    }

    public async Task<PlayerDto> CreateAsync(PlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Player player = ToDomain(dto);
        player.Id = Guid.NewGuid();
        player.CreatedAt = DateTime.UtcNow;
        Player created = await _playerRepository.CreateAsync(player).ConfigureAwait(false);
        return ToDto(created);
    }

    public async Task<PlayerDto?> UpdateAsync(Guid id, PlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Player? existing = await _playerRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null)
            return null;

        Player updated = ToDomain(dto);
        updated.Id = id;
        updated.CreatedAt = existing.CreatedAt;
        Player saved = await _playerRepository.UpdateAsync(updated).ConfigureAwait(false);
        return ToDto(saved);
    }

    public Task<bool> DeleteAsync(Guid id) =>
        _playerRepository.DeleteAsync(id);

    private static PlayerDto ToDto(Player p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Nationality = p.Nationality,
        Ranking = p.Ranking,
        CreatedAt = p.CreatedAt,
    };

    private static Player ToDomain(PlayerDto dto) => new()
    {
        Name = dto.Name ?? string.Empty,
        Nationality = dto.Nationality,
        Ranking = dto.Ranking,
    };
}
