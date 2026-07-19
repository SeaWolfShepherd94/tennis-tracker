using TennisTracker.Api.Interfaces;
using TennisTracker.Api.Services;
using TennisTracker.Data;
using TennisTracker.Data.Interfaces;
using TennisTracker.Data.Repositories;

namespace TennisTracker.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTennisTrackerServices(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseInitializer>();

        services.AddScoped<IHealthzService, HealthzService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<IPlayerService, PlayerService>();

        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();

        return services;
    }

    public static IServiceCollection BindSettings<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName) where T : class
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.Configure<T>(configuration.GetSection(sectionName));
        return services;
    }
}
