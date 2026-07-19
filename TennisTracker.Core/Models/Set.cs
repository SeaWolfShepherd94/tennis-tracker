namespace TennisTracker.Core.Models;

public sealed class MatchSet
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public int SetNumber { get; set; }
    public int PlayerOneGames { get; set; }
    public int PlayerTwoGames { get; set; }
    public int? PlayerOneTiebreakPoints { get; set; }
    public int? PlayerTwoTiebreakPoints { get; set; }
}
