namespace TennisTracker.Api.Interfaces;

public interface IHealthzService
{
    Task<bool> IsHealthyAsync();
}
