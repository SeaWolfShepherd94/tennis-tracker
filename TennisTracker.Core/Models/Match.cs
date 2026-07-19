using TennisTracker.Core.Enumerations;

namespace TennisTracker.Core.Models;

public sealed class Match
{
    public Guid Id { get; set; }
    public Guid PlayerOneId { get; set; }
    public Guid PlayerTwoId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Surface Surface { get; set; }
    public MatchStatus Status { get; set; }
    public Guid? WinnerId { get; set; }
    public IList<MatchSet> Sets { get; } = [];
}
