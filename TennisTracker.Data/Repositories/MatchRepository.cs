using System.Data.Common;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TennisTracker.Core.Models;
using TennisTracker.Data.Interfaces;
using TennisTracker.Data.Options;

namespace TennisTracker.Data.Repositories;

public sealed class MatchRepository(IOptions<DatabaseOptions> options) : IMatchRepository
{
    private readonly DatabaseOptions _options = options.Value;

    private SqliteConnection CreateConnection() => new(_options.ConnectionString);

    public async Task<IEnumerable<Match>> GetAllAsync()
    {
        using SqliteConnection db = CreateConnection();
        List<Match> matches = (await db.QueryAsync<Match>(
            "SELECT * FROM Matches ORDER BY StartTime DESC").ConfigureAwait(false)).ToList();

        if (matches.Count == 0) return matches;

        IEnumerable<MatchSet> sets = await db.QueryAsync<MatchSet>(
            "SELECT * FROM Sets WHERE MatchId IN @Ids ORDER BY SetNumber",
            new { Ids = matches.Select(m => m.Id.ToString()).ToList() }).ConfigureAwait(false);

        Dictionary<Guid, List<MatchSet>> setsByMatch = sets
            .GroupBy(s => s.MatchId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (Match match in matches)
        {
            if (setsByMatch.TryGetValue(match.Id, out List<MatchSet>? matchSets))
                foreach (MatchSet s in matchSets) match.Sets.Add(s);
        }

        return matches;
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        using SqliteConnection db = CreateConnection();
        Match? match = await db.QuerySingleOrDefaultAsync<Match>(
            "SELECT * FROM Matches WHERE Id = @Id",
            new { Id = id.ToString() }).ConfigureAwait(false);

        if (match is null) return null;

        IEnumerable<MatchSet> sets = await db.QueryAsync<MatchSet>(
            "SELECT * FROM Sets WHERE MatchId = @MatchId ORDER BY SetNumber",
            new { MatchId = id.ToString() }).ConfigureAwait(false);

        foreach (MatchSet s in sets) match.Sets.Add(s);
        return match;
    }

    public async Task<Match> CreateAsync(Match match)
    {
        ArgumentNullException.ThrowIfNull(match);

        using SqliteConnection db = CreateConnection();
        await db.OpenAsync().ConfigureAwait(false);
        using SqliteTransaction tx = (SqliteTransaction)await db.BeginTransactionAsync().ConfigureAwait(false);

        await db.ExecuteAsync(
            """
            INSERT INTO Matches (Id, PlayerOneId, PlayerTwoId, StartTime, EndTime, Surface, Status, WinnerId)
            VALUES (@Id, @PlayerOneId, @PlayerTwoId, @StartTime, @EndTime, @Surface, @Status, @WinnerId)
            """,
            new
            {
                Id = match.Id.ToString(),
                PlayerOneId = match.PlayerOneId.ToString(),
                PlayerTwoId = match.PlayerTwoId.ToString(),
                match.StartTime,
                match.EndTime,
                Surface = match.Surface.ToString(),
                Status = match.Status.ToString(),
                WinnerId = match.WinnerId?.ToString(),
            }, tx).ConfigureAwait(false);

        await InsertSetsAsync(db, match, tx).ConfigureAwait(false);

        await tx.CommitAsync().ConfigureAwait(false);
        return match;
    }

    public async Task<Match> UpdateAsync(Match match)
    {
        ArgumentNullException.ThrowIfNull(match);

        using SqliteConnection db = CreateConnection();
        await db.OpenAsync().ConfigureAwait(false);
        using SqliteTransaction tx = (SqliteTransaction)await db.BeginTransactionAsync().ConfigureAwait(false);

        await db.ExecuteAsync(
            """
            UPDATE Matches
            SET PlayerOneId = @PlayerOneId, PlayerTwoId = @PlayerTwoId,
                StartTime = @StartTime, EndTime = @EndTime,
                Surface = @Surface, Status = @Status, WinnerId = @WinnerId
            WHERE Id = @Id
            """,
            new
            {
                Id = match.Id.ToString(),
                PlayerOneId = match.PlayerOneId.ToString(),
                PlayerTwoId = match.PlayerTwoId.ToString(),
                match.StartTime,
                match.EndTime,
                Surface = match.Surface.ToString(),
                Status = match.Status.ToString(),
                WinnerId = match.WinnerId?.ToString(),
            }, tx).ConfigureAwait(false);

        await db.ExecuteAsync(
            "DELETE FROM Sets WHERE MatchId = @MatchId",
            new { MatchId = match.Id.ToString() }, tx).ConfigureAwait(false);

        await InsertSetsAsync(db, match, tx).ConfigureAwait(false);

        await tx.CommitAsync().ConfigureAwait(false);
        return match;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using SqliteConnection db = CreateConnection();
        await db.OpenAsync().ConfigureAwait(false);
        using SqliteTransaction tx = (SqliteTransaction)await db.BeginTransactionAsync().ConfigureAwait(false);

        await db.ExecuteAsync(
            "DELETE FROM Sets WHERE MatchId = @MatchId",
            new { MatchId = id.ToString() }, tx).ConfigureAwait(false);

        int rows = await db.ExecuteAsync(
            "DELETE FROM Matches WHERE Id = @Id",
            new { Id = id.ToString() }, tx).ConfigureAwait(false);

        await tx.CommitAsync().ConfigureAwait(false);
        return rows > 0;
    }

    private static async Task InsertSetsAsync(SqliteConnection db, Match match, DbTransaction tx)
    {
        foreach (MatchSet set in match.Sets)
        {
            set.Id = Guid.NewGuid();
            set.MatchId = match.Id;
            await db.ExecuteAsync(
                """
                INSERT INTO Sets (Id, MatchId, SetNumber, PlayerOneGames, PlayerTwoGames,
                                  PlayerOneTiebreakPoints, PlayerTwoTiebreakPoints)
                VALUES (@Id, @MatchId, @SetNumber, @PlayerOneGames, @PlayerTwoGames,
                        @PlayerOneTiebreakPoints, @PlayerTwoTiebreakPoints)
                """,
                new
                {
                    Id = set.Id.ToString(),
                    MatchId = set.MatchId.ToString(),
                    set.SetNumber,
                    set.PlayerOneGames,
                    set.PlayerTwoGames,
                    set.PlayerOneTiebreakPoints,
                    set.PlayerTwoTiebreakPoints,
                }, tx).ConfigureAwait(false);
        }
    }
}
