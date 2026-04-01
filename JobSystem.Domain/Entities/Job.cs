namespace JobSystem.Domain.Entities;

public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = String.Empty;
    public string Payload { get; set; } = String.Empty;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public int RetryCount { get; set; } = 0;

}