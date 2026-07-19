using Serilog;

namespace TennisTracker.Api;

public sealed class Program
{
    public static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration))
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .Build()
            .Run();
}
