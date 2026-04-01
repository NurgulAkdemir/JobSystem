using JobSystem.Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace JobSystem.Infrastructure.Workers;

public class JobWorker: BackgroundService
{
    private readonly IJobQueue _jobQueue;

    public JobWorker(IJobQueue jobQueue)
    {
        _jobQueue = jobQueue;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken){
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_jobQueue.TryDequeue(out var job))
            {
                Console.WriteLine($"Job işlendi:{job.Id} - {job.Type}");
            }
            
            await Task.Delay(1000, stoppingToken);
            
        }
    }
}