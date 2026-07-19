namespace TennisTracker.Api.Models;

public sealed class PlayerDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Nationality { get; set; }
    public int? Ranking { get; set; }
    public DateTime? CreatedAt { get; set; }
}
