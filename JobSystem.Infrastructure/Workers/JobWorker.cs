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

    //  Concurrency kontrolü
    private readonly SemaphoreSlim _semaphore = new(3);

    public JobWorker(IJobQueue jobQueue, IEnumerable<IJobHandler> handlers, IDeadLetterQueue deadLetterQueue, ILogger<JobWorker> logger)
    {
        _jobQueue = jobQueue;
        _handlers = handlers;
        _deadLetterQueue = deadLetterQueue;
        _logger = logger;
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
            var handler = _handlers.FirstOrDefault(h => h.JobType == job.Type);

            if (handler != null)
            {
                using (LogContext.PushProperty("JobId", job.Id))
                {
                    _logger.LogInformation("Job başladı");
                    await handler.HandleAsync(job);
                    _logger.LogInformation("Job bitti");
                }
            }
            else
            {
                _logger.LogInformation($"Handler bulunamadı: {job.Type}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Hata: {ex.Message}");

            job.RetryCount++;

            if (job.RetryCount < 3)
            {
                _logger.LogWarning("Retry ediliyor: {JobId}", job.Id);
                _jobQueue.Enqueue(job);
            }
            else
            {
                _logger.LogError("Job DLQ'ya gönderildi: {JobId}", job.Id);
                _deadLetterQueue.Enqueue(job);  // DLQ'ya gönder
            }
        }
        finally
        {
           
            _semaphore.Release();
        }
    }
}