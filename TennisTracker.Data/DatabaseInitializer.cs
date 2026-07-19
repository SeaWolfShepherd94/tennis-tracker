using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TennisTracker.Data.Options;

namespace TennisTracker.Data;

public sealed class DatabaseInitializer(IOptions<DatabaseOptions> options)
{
    private readonly string _connectionString = options.Value.ConnectionString;

    public void Initialize()
    {
        SqliteTypeHandlers.Register();

        using var db = new SqliteConnection(_connectionString);
        db.Open();

        db.Execute("""
            CREATE TABLE IF NOT EXISTS Players (
                Id          TEXT PRIMARY KEY NOT NULL,
                Name        TEXT NOT NULL,
                Nationality TEXT,
                Ranking     INTEGER,
                CreatedAt   TEXT NOT NULL
            );
            """);

        db.Execute("""
            CREATE TABLE IF NOT EXISTS Matches (
                Id           TEXT PRIMARY KEY NOT NULL,
                PlayerOneId  TEXT NOT NULL,
                PlayerTwoId  TEXT NOT NULL,
                StartTime    TEXT NOT NULL,
                EndTime      TEXT,
                Surface      TEXT NOT NULL,
                Status       TEXT NOT NULL
            );
            """);

        db.Execute("""
            CREATE TABLE IF NOT EXISTS Sets (
                Id                       TEXT PRIMARY KEY NOT NULL,
                MatchId                  TEXT NOT NULL,
                SetNumber                INTEGER NOT NULL,
                PlayerOneGames           INTEGER NOT NULL,
                PlayerTwoGames           INTEGER NOT NULL,
                PlayerOneTiebreakPoints  INTEGER,
                PlayerTwoTiebreakPoints  INTEGER
            );
            """);

        IEnumerable<dynamic> matchCols = db.Query("PRAGMA table_info(Matches)");
        if (!matchCols.Any(c => (string)c.name == "WinnerId"))
            db.Execute("ALTER TABLE Matches ADD COLUMN WinnerId TEXT");
    }
}
