using TennisTracker.Api.Models;

namespace TennisTracker.Api.Interfaces;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetAllAsync();
    Task<MatchDto?> GetByIdAsync(Guid id);
    Task<MatchDto> CreateAsync(MatchDto dto);
    Task<MatchDto?> UpdateAsync(Guid id, MatchDto dto);
    Task<MatchDto?> SaveScoreAsync(Guid id, ScoreRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<int> ImportWimbledonDrawAsync();
}
