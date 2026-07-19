using Microsoft.AspNetCore.Diagnostics;
using TennisTracker.Api.Extensions;
using TennisTracker.Data;
using TennisTracker.Data.Options;

namespace TennisTracker.Api;

public sealed class Startup(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration
        ?? throw new ArgumentNullException(nameof(configuration));

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()));

        services.BindSettings<DatabaseOptions>(_configuration, DatabaseOptions.Section);
        services.AddTennisTrackerServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseInitializer dbInit)
    {
        ArgumentNullException.ThrowIfNull(dbInit);
        dbInit.Initialize();

        app.UseExceptionHandler(a => a.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            await context.Response.WriteAsJsonAsync(new { error = feature?.Error.Message ?? "An unexpected error occurred." }).ConfigureAwait(false);
        }));

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
