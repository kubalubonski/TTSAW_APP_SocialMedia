using Microsoft.EntityFrameworkCore;

namespace PeopleApi.Configuration;

internal static class StartupSupport
{
    public static string ResolveConnectionString(IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionName = environment.IsDevelopment()
            ? "DefaultConnection"
            : "DockerConnection";

        var connectionString = configuration.GetConnectionString(connectionName);

        if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("SET_DB_PASSWORD_IN_ENV", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Missing or unresolved connection string '{connectionName}'. Configure it via environment variables or .env-based startup.");
        }

        return connectionString;
    }

    public static string GetRequiredSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "SET_JWT_SECRET_IN_ENV", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Missing required configuration '{key}'. Configure it via environment variables or .env-based startup.");
        }

        return value;
    }

    public static async Task MigrateDatabaseAsync<TContext>(IServiceProvider services, ILogger logger)
        where TContext : DbContext
    {
        const int maxAttempts = 10;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("Applied migrations for {DbContext}.", typeof(TContext).Name);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Migration attempt {Attempt}/{MaxAttempts} failed for {DbContext}.", attempt, maxAttempts, typeof(TContext).Name);

                if (attempt == maxAttempts)
                {
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}