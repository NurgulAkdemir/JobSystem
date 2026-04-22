using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace JobSystem.Infrastructure.Workers;

public class JobWorker : BackgroundService
{
    private readonly IJobQueue _jobQueue;
    private readonly IEnumerable<IJobHandler> _handlers;
    private readonly IDeadLetterQueue _deadLetterQueue;
    private readonly ILogger<JobWorker> _logger;
    private readonly IJobMetricsService _metrics;

    //  Concurrency kontrolü
    private readonly SemaphoreSlim _semaphore = new(3);

    public JobWorker(IJobQueue jobQueue, IEnumerable<IJobHandler> handlers, IDeadLetterQueue deadLetterQueue, ILogger<JobWorker> logger,IJobMetricsService metrics)
    {
        _jobQueue = jobQueue;
        _handlers = handlers;
        _deadLetterQueue = deadLetterQueue;
        _logger = logger;
        _metrics = metrics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker başladı");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_jobQueue.TryDequeue(out var job))
            {
                // fire-and-forget ama kontrollü
                _ = ProcessJobAsync(job, stoppingToken);
            }

            await Task.Delay(200, stoppingToken);
        }
    }

    //  BU METOT AYNI DOSYADA OLACAK
    private async Task ProcessJobAsync(Job job, CancellationToken stoppingToken)
    {
        await _semaphore.WaitAsync(stoppingToken);

        try
        {
            // ✅ 1. TOTAL JOB (HER PROCESS BAŞINDA)
            _metrics.IncrementTotal();

            var handler = _handlers.FirstOrDefault(h => h.JobType == job.Type);

            if (handler != null)
            {
                using (Serilog.Context.LogContext.PushProperty("JobId", job.Id))
                {
                    _logger.LogInformation("Job başladı");

                    await handler.HandleAsync(job);

                    // ✅ 2. SUCCESS (SADECE HATA YOKSA)
                    _metrics.IncrementSuccess();

                    _logger.LogInformation("Job bitti");
                }
            }
            else
            {
                _logger.LogWarning("Handler bulunamadı: {JobType}", job.Type);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hata oluştu: {JobId}", job.Id);

            job.RetryCount++;

            if (job.RetryCount < 3)
            {
                // ✅ 3. RETRY SAY
                _metrics.IncrementRetry();

                _logger.LogWarning("Retry ediliyor: {JobId}", job.Id);

                _jobQueue.Enqueue(job);
            }
            else
            {
                // ✅ 4. FAILED (DLQ'ya giderse)
                _metrics.IncrementFailed();

                _logger.LogError("Job DLQ'ya gönderildi: {JobId}", job.Id);

                _deadLetterQueue.Enqueue(job);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
