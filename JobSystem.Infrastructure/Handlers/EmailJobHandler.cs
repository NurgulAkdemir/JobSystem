using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using System.Threading.Tasks;

namespace JobSystem.Infrastructure.Handlers;

public class EmailJobHandler : IJobHandler
{
    public string JobType => "Email";

    public async Task HandleAsync(Job job)
    {
        Console.WriteLine($"Email işleniyor: {job.Id}");

        await Task.Delay(3000); // paralelliği görmek için
    }
}