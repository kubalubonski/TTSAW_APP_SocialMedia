namespace SocialMediaApp.Configuration;

internal static class StartupSupport
{
    public static string GetRequiredSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];

        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "SET_JWT_SECRET_IN_ENV", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Missing required configuration '{key}'. Configure it via environment variables or .env-based startup.");
        }

        return value;
    }
}