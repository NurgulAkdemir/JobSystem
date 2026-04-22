using JobSystem.Domain.Entities;

namespace JobSystem.Application.Interfaces;

public interface IJobMetricsService
{
    void IncrementTotal();
    void IncrementSuccess();
    void IncrementFailed();
    void IncrementRetry();

    JobMetrics GetMetrics();
}