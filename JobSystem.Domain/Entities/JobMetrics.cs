namespace JobSystem.Domain.Entities;

public class JobMetrics
{
    public int TotalJobs { get; set; }
    public int SuccessJobs { get; set; }
    public int FailedJobs { get; set; }
    public int RetriedJobs { get; set; }
}