using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace JobSystem.Infrastructure.Workers;

public class JobWorker : BackgroundService
{
    private readonly IJobQueue _jobQueue;
    private readonly IEnumerable<IJobHandler> _handlers;

    //  Concurrency kontrolü
    private readonly SemaphoreSlim _semaphore = new(3);

    public JobWorker(IJobQueue jobQueue, IEnumerable<IJobHandler> handlers)
    {
        _jobQueue = jobQueue;
        _handlers = handlers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Worker başladı");

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
                Console.WriteLine($"Job başladı: {job.Id}");

                await handler.HandleAsync(job);

                Console.WriteLine($"Job bitti: {job.Id}");
            }
            else
            {
                Console.WriteLine($"Handler bulunamadı: {job.Type}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");

            job.RetryCount++;

            if (job.RetryCount < 3)
            {
                Console.WriteLine($"Retry: {job.Id}");
                _jobQueue.Enqueue(job);
            }
            else
            {
                Console.WriteLine($"Job öldü: {job.Id}");
            }
        }
        finally
        {
            //  EN KRİTİK SATIR
            _semaphore.Release();
        }
    }
}