namespace TennisTracker.Api.Models;

public sealed class SetDto
{
    public int SetNumber { get; set; }
    public int PlayerOneGames { get; set; }
    public int PlayerTwoGames { get; set; }
    public int? PlayerOneTiebreakPoints { get; set; }
    public int? PlayerTwoTiebreakPoints { get; set; }
}
