using TennisTracker.Api.Models;

namespace TennisTracker.Api.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
    Task<PlayerDto?> GetByIdAsync(Guid id);
    Task<PlayerDto> CreateAsync(PlayerDto dto);
    Task<PlayerDto?> UpdateAsync(Guid id, PlayerDto dto);
    Task<bool> DeleteAsync(Guid id);
}
