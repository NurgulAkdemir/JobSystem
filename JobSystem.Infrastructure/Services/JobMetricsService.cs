using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;

namespace JobSystem.Infrastructure.Services;

public class JobMetricsService : IJobMetricsService
{
    private readonly JobMetrics _metrics = new();

    public void IncrementTotal()=> _metrics.TotalJobs++;
    public void IncrementSuccess()=> _metrics.SuccessJobs++;
    public void IncrementFailed()=> _metrics.FailedJobs++;
    public void IncrementRetry()=> _metrics.RetriedJobs++;

    public JobMetrics GetMetrics()=> _metrics;

    public void Reset()
    {
        _metrics.TotalJobs = 0;
        _metrics.SuccessJobs = 0;
        _metrics.FailedJobs = 0;
        _metrics.RetriedJobs = 0;
    }
}