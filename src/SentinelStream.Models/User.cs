namespace SentinelStream.Models;

/// <summary>
/// Represents a SOC team user.
/// </summary>
public class User
{
    public string UserId { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string DisplayName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Analyst;
}

public enum UserRole
{
    Admin,
    Lead,
    Analyst,
    Observer
}
