using TennisTracker.Api.Interfaces;

namespace TennisTracker.Api.Services;

public sealed class HealthzService : IHealthzService
{
    public Task<bool> IsHealthyAsync() => Task.FromResult(true);
}
