namespace NotificationService.Model;
using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Number { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public List<Notification>? Notifications { get; set; } = new();

}