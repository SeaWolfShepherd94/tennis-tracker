namespace TennisTracker.Api.Models;

public sealed class ScoreRequest
{
    public IList<SetDto> Sets { get; init; } = [];
}
