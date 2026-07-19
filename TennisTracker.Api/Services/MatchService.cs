using TennisTracker.Api.Interfaces;
using TennisTracker.Api.Models;
using TennisTracker.Core.Enumerations;
using TennisTracker.Core.Models;
using TennisTracker.Data.Interfaces;

namespace TennisTracker.Api.Services;

public sealed class MatchService(IMatchRepository matchRepository, IPlayerRepository playerRepository) : IMatchService
{
    private readonly IMatchRepository _matchRepository = matchRepository
        ?? throw new ArgumentNullException(nameof(matchRepository));
    private readonly IPlayerRepository _playerRepository = playerRepository
        ?? throw new ArgumentNullException(nameof(playerRepository));

    public async Task<IEnumerable<MatchDto>> GetAllAsync()
    {
        IEnumerable<Match> matches = await _matchRepository.GetAllAsync().ConfigureAwait(false);
        Dictionary<Guid, Player> players = (await _playerRepository.GetAllAsync().ConfigureAwait(false))
            .ToDictionary(p => p.Id);
        return matches.Select(m => ToDto(m, players));
    }

    public async Task<MatchDto?> GetByIdAsync(Guid id)
    {
        Match? match = await _matchRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (match is null) return null;
        Dictionary<Guid, Player> players = (await _playerRepository.GetAllAsync().ConfigureAwait(false))
            .ToDictionary(p => p.Id);
        return ToDto(match, players);
    }

    public async Task<MatchDto> CreateAsync(MatchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Match match = ToDomain(dto);
        match.Id = Guid.NewGuid();
        Match created = await _matchRepository.CreateAsync(match).ConfigureAwait(false);
        Dictionary<Guid, Player> players = (await _playerRepository.GetAllAsync().ConfigureAwait(false))
            .ToDictionary(p => p.Id);
        return ToDto(created, players);
    }

    public async Task<MatchDto?> UpdateAsync(Guid id, MatchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Match? existing = await _matchRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null) return null;

        Match updated = ToDomain(dto);
        updated.Id = id;
        Match saved = await _matchRepository.UpdateAsync(updated).ConfigureAwait(false);
        Dictionary<Guid, Player> players = (await _playerRepository.GetAllAsync().ConfigureAwait(false))
            .ToDictionary(p => p.Id);
        return ToDto(saved, players);
    }

    public async Task<MatchDto?> SaveScoreAsync(Guid id, ScoreRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Sets.Count == 0)
            throw new ArgumentException("At least one set is required.");

        Match? existing = await _matchRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null) return null;

        List<MatchSet> sets = request.Sets.Select((s, i) => new MatchSet
        {
            SetNumber = s.SetNumber > 0 ? s.SetNumber : i + 1,
            PlayerOneGames = s.PlayerOneGames,
            PlayerTwoGames = s.PlayerTwoGames,
            PlayerOneTiebreakPoints = s.PlayerOneTiebreakPoints,
            PlayerTwoTiebreakPoints = s.PlayerTwoTiebreakPoints,
        }).ToList();

        existing.Sets.Clear();
        foreach (MatchSet s in sets) existing.Sets.Add(s);
        existing.WinnerId = ComputeWinner(existing.PlayerOneId, existing.PlayerTwoId, sets);
        existing.Status = MatchStatus.Completed;
        existing.EndTime = DateTime.UtcNow;

        Match saved = await _matchRepository.UpdateAsync(existing).ConfigureAwait(false);
        Dictionary<Guid, Player> players = (await _playerRepository.GetAllAsync().ConfigureAwait(false))
            .ToDictionary(p => p.Id);
        return ToDto(saved, players);
    }

    public Task<bool> DeleteAsync(Guid id) =>
        _matchRepository.DeleteAsync(id);

    public async Task<int> ImportWimbledonDrawAsync()
    {
        IEnumerable<Player> existingPlayers = await _playerRepository.GetAllAsync().ConfigureAwait(false);
        Dictionary<string, Player> playersByName = existingPlayers
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        DateTime firstRoundStart = new(2026, 6, 30, 11, 0, 0, DateTimeKind.Utc);
        int matchesCreated = 0;

        foreach (DrawMatchup entry in _firstRoundMatchups)
        {
            Player p1 = await GetOrCreatePlayerAsync(playersByName, entry.P1Name, entry.P1Nationality).ConfigureAwait(false);
            Player p2 = await GetOrCreatePlayerAsync(playersByName, entry.P2Name, entry.P2Nationality).ConfigureAwait(false);

            Match match = new()
            {
                Id = Guid.NewGuid(),
                PlayerOneId = p1.Id,
                PlayerTwoId = p2.Id,
                StartTime = firstRoundStart,
                Surface = Surface.Grass,
                Status = MatchStatus.Scheduled,
            };

            await _matchRepository.CreateAsync(match).ConfigureAwait(false);
            matchesCreated++;
        }

        return matchesCreated;
    }

    private async Task<Player> GetOrCreatePlayerAsync(
        Dictionary<string, Player> cache, string name, string nationality)
    {
        if (cache.TryGetValue(name, out Player? existing))
            return existing;

        Player player = new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Nationality = nationality,
            CreatedAt = DateTime.UtcNow,
        };

        Player created = await _playerRepository.CreateAsync(player).ConfigureAwait(false);
        cache[name] = created;
        return created;
    }

    private readonly record struct DrawMatchup(string P1Name, string P1Nationality, string P2Name, string P2Nationality);

    private static readonly DrawMatchup[] _firstRoundMatchups =
    [
        new("Jannik Sinner", "ITA", "Yannick Hanfmann", "GER"),
        new("Holger Rune", "DEN", "Luca Nardi", "ITA"),
        new("Jack Draper", "GBR", "Giovanni Mpetshi Perricard", "FRA"),
        new("Taylor Fritz", "USA", "Jordan Thompson", "AUS"),
        new("Stefanos Tsitsipas", "GRE", "Tallon Griekspoor", "NED"),
        new("Ben Shelton", "USA", "Luciano Darderi", "ITA"),
        new("Casper Ruud", "NOR", "Nicolas Jarry", "CHI"),
        new("Karen Khachanov", "RUS", "Borna Coric", "CRO"),
        new("Daniil Medvedev", "RUS", "Quentin Halys", "FRA"),
        new("Cameron Norrie", "GBR", "Corentin Moutet", "FRA"),
        new("Flavio Cobolli", "ITA", "Roman Safiullin", "RUS"),
        new("Lorenzo Musetti", "ITA", "Ugo Humbert", "FRA"),
        new("Tommy Paul", "USA", "Brandon Nakashima", "USA"),
        new("Alex de Minaur", "AUS", "Jiri Lehecka", "CZE"),
        new("Frances Tiafoe", "USA", "Jakub Mensik", "CZE"),
        new("Carlos Alcaraz", "ESP", "Laslo Djere", "SRB"),
        new("Novak Djokovic", "SRB", "Jan-Lennard Struff", "GER"),
        new("Sebastian Korda", "USA", "Arthur Cazaux", "FRA"),
        new("Matteo Berrettini", "ITA", "Alexei Popyrin", "AUS"),
        new("Andrey Rublev", "RUS", "Felix Auger-Aliassime", "CAN"),
        new("Hubert Hurkacz", "POL", "Tomas Machac", "CZE"),
        new("Denis Shapovalov", "CAN", "Nuno Borges", "POR"),
        new("Arthur Fils", "FRA", "Christopher Eubanks", "USA"),
        new("Alexander Zverev", "GER", "Botic van de Zandschulp", "NED"),
        new("Grigor Dimitrov", "BUL", "Tomas Etcheverry", "ARG"),
        new("Sebastian Baez", "ARG", "Alejandro Davidovich Fokina", "ESP"),
        new("Miomir Kecmanovic", "SRB", "Roberto Bautista Agut", "ESP"),
        new("Juncheng Shang", "CHN", "Pedro Martinez", "ESP"),
        new("Mariano Navone", "ARG", "Hamad Medjedovic", "SRB"),
        new("Cristian Garin", "CHI", "Borna Gojo", "CRO"),
        new("Alexandre Muller", "FRA", "Daniel Evans", "GBR"),
        new("Matteo Arnaldi", "ITA", "Otto Virtanen", "FIN"),
    ];

    private static Guid? ComputeWinner(Guid playerOneId, Guid playerTwoId, IList<MatchSet> sets)
    {
        int p1Sets = 0, p2Sets = 0;
        foreach (MatchSet s in sets)
        {
            bool p1Wins = s.PlayerOneGames > s.PlayerTwoGames
                || (s.PlayerOneGames == s.PlayerTwoGames
                    && s.PlayerOneTiebreakPoints > s.PlayerTwoTiebreakPoints);
            if (p1Wins) p1Sets++; else p2Sets++;
        }
        if (p1Sets == 3) return playerOneId;
        if (p2Sets == 3) return playerTwoId;
        return null;
    }

    private static MatchDto ToDto(Match m, Dictionary<Guid, Player> players) => new()
    {
        Id = m.Id,
        PlayerOneId = m.PlayerOneId,
        PlayerTwoId = m.PlayerTwoId,
        PlayerOneName = players.GetValueOrDefault(m.PlayerOneId)?.Name,
        PlayerTwoName = players.GetValueOrDefault(m.PlayerTwoId)?.Name,
        StartTime = m.StartTime,
        EndTime = m.EndTime,
        Surface = m.Surface.ToString(),
        Status = m.Status.ToString(),
        WinnerId = m.WinnerId,
        WinnerName = m.WinnerId.HasValue ? players.GetValueOrDefault(m.WinnerId.Value)?.Name : null,
        Sets = m.Sets.Select((s, i) => new SetDto
        {
            SetNumber = s.SetNumber > 0 ? s.SetNumber : i + 1,
            PlayerOneGames = s.PlayerOneGames,
            PlayerTwoGames = s.PlayerTwoGames,
            PlayerOneTiebreakPoints = s.PlayerOneTiebreakPoints,
            PlayerTwoTiebreakPoints = s.PlayerTwoTiebreakPoints,
        }).ToList(),
    };

    private static Match ToDomain(MatchDto dto)
    {
        Match match = new()
        {
            PlayerOneId = dto.PlayerOneId ?? Guid.Empty,
            PlayerTwoId = dto.PlayerTwoId ?? Guid.Empty,
            StartTime = dto.StartTime ?? DateTime.UtcNow,
            EndTime = dto.EndTime,
            Surface = Enum.TryParse<Surface>(dto.Surface, out Surface surface) ? surface : Surface.Hard,
            Status = Enum.TryParse<MatchStatus>(dto.Status, out MatchStatus status) ? status : MatchStatus.Scheduled,
        };

        if (dto.Sets is not null)
        {
            int i = 0;
            foreach (SetDto s in dto.Sets)
            {
                match.Sets.Add(new MatchSet
                {
                    SetNumber = s.SetNumber > 0 ? s.SetNumber : ++i,
                    PlayerOneGames = s.PlayerOneGames,
                    PlayerTwoGames = s.PlayerTwoGames,
                    PlayerOneTiebreakPoints = s.PlayerOneTiebreakPoints,
                    PlayerTwoTiebreakPoints = s.PlayerTwoTiebreakPoints,
                });
            }
        }

        return match;
    }
}
