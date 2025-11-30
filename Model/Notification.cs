namespace NotificationService.Model;
using System.Text.Json.Serialization;

public class Notification
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Status { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public int UserId { get; set; }

    [JsonIgnore]
    public User? User { get; set; }   
}