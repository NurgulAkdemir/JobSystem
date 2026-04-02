using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using Microsoft.Extensions.Hosting;

namespace JobSystem.Infrastructure.Workers;

public class JobWorker : BackgroundService
{
    private readonly IJobQueue _jobQueue;
    private readonly IEnumerable<IJobHandler> _handlers;

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
            if (_jobQueue.TryDequeue(out Job job))
            {
                var handler = _handlers.FirstOrDefault(h => h.JobType == job.Type);

                if (handler != null)
                {
                    await handler.HandleAsync(job);
                }
                else
                {
                    Console.WriteLine($"Handler bulunamadı: {job.Type}");
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}