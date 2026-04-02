using SentinelStream.Models;

namespace SentinelStream.Services;

/// <summary>
/// Loads configuration from .env file into AppConfig.
/// </summary>
public static class ConfigLoader
{
    /// <summary>
    /// Loads the .env file from the given path and returns an AppConfig.
    /// </summary>
    public static AppConfig LoadFromEnvFile(string envFilePath = ".env")
    {
        var config = new AppConfig();

        if (!File.Exists(envFilePath))
        {
            // Try looking relative to the executable
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            envFilePath = Path.Combine(exeDir, ".env");

            if (!File.Exists(envFilePath))
                throw new FileNotFoundException(
                    "Configuration file .env not found. Run setup_env.ps1 first.", envFilePath);
        }

        var lines = File.ReadAllLines(envFilePath);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                continue;

            var separatorIndex = trimmed.IndexOf('=');
            if (separatorIndex <= 0) continue;

            var key = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim();

            switch (key)
            {
                case "AGORA_APP_ID":
                    config.AgoraAppId = value;
                    break;
                case "AGORA_APP_CERTIFICATE":
                    config.AgoraAppCertificate = value;
                    break;
                case "ENCRYPTION_KEY":
                    config.EncryptionKey = value;
                    break;
                case "FORENSIC_SALT":
                    config.ForensicSalt = value;
                    break;
                case "LOG_SERVER_URL":
                    config.LogServerUrl = value;
                    break;
            }
        }

        return config;
    }
}
