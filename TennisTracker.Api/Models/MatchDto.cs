namespace TennisTracker.Api.Models;

public sealed class MatchDto
{
    public Guid? Id { get; set; }
    public Guid? PlayerOneId { get; set; }
    public Guid? PlayerTwoId { get; set; }
    public string? PlayerOneName { get; set; }
    public string? PlayerTwoName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Surface { get; set; }
    public string? Status { get; set; }
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public IList<SetDto>? Sets { get; init; }
}
