using TennisTracker.Core.Models;

namespace TennisTracker.Data.Interfaces;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(Guid id);
    Task<Player> CreateAsync(Player player);
    Task<Player> UpdateAsync(Player player);
    Task<bool> DeleteAsync(Guid id);
}
