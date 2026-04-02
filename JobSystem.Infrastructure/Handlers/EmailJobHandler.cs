using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using System.Threading.Tasks;

namespace JobSystem.Infrastructure.Handlers;

public class EmailJobHandler : IJobHandler
{
    public string JobType => "Email";

    public Task HandleAsync(Job job)
    {
        Console.WriteLine($"📧 Email gönderildi: {job.Payload}");
        return Task.CompletedTask;
    }
}