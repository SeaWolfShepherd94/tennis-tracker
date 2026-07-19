namespace TennisTracker.Core.Models;

public sealed class Player
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Nationality { get; set; }
    public int? Ranking { get; set; }
    public DateTime CreatedAt { get; set; }
}
