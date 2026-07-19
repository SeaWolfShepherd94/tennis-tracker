using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TennisTracker.Core.Models;
using TennisTracker.Data.Interfaces;
using TennisTracker.Data.Options;

namespace TennisTracker.Data.Repositories;

public sealed class PlayerRepository(IOptions<DatabaseOptions> options) : IPlayerRepository
{
    private readonly DatabaseOptions _options = options.Value;

    private SqliteConnection CreateConnection() => new(_options.ConnectionString);

    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        using SqliteConnection db = CreateConnection();
        return await db.QueryAsync<Player>("SELECT * FROM Players ORDER BY Name").ConfigureAwait(false);
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        using SqliteConnection db = CreateConnection();
        return await db.QuerySingleOrDefaultAsync<Player>(
            "SELECT * FROM Players WHERE Id = @Id",
            new { Id = id.ToString() }).ConfigureAwait(false);
    }

    public async Task<Player> CreateAsync(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        using SqliteConnection db = CreateConnection();
        await db.ExecuteAsync(
            """
            INSERT INTO Players (Id, Name, Nationality, Ranking, CreatedAt)
            VALUES (@Id, @Name, @Nationality, @Ranking, @CreatedAt)
            """,
            new
            {
                Id = player.Id.ToString(),
                player.Name,
                player.Nationality,
                player.Ranking,
                player.CreatedAt,
            }).ConfigureAwait(false);
        return player;
    }

    public async Task<Player> UpdateAsync(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        using SqliteConnection db = CreateConnection();
        await db.ExecuteAsync(
            """
            UPDATE Players
            SET Name = @Name, Nationality = @Nationality, Ranking = @Ranking
            WHERE Id = @Id
            """,
            new
            {
                Id = player.Id.ToString(),
                player.Name,
                player.Nationality,
                player.Ranking,
            }).ConfigureAwait(false);
        return player;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using SqliteConnection db = CreateConnection();
        int rows = await db.ExecuteAsync(
            "DELETE FROM Players WHERE Id = @Id",
            new { Id = id.ToString() }).ConfigureAwait(false);
        return rows > 0;
    }
}
