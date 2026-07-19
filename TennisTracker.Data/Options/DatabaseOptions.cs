namespace TennisTracker.Data.Options;

public sealed class DatabaseOptions
{
    public const string Section = "DatabaseOptions";
    public string ConnectionString { get; set; } = "Data Source=tennis_tracker.db";
}
