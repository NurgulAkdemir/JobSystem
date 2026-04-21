using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JobSystem.Infrastructure.Handlers;

public class EmailJobHandler : IJobHandler
{
    public string JobType => "Email";
    private readonly ILogger<EmailJobHandler> _logger;

    public EmailJobHandler(ILogger<EmailJobHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(Job job)
    {
        _logger.LogInformation("Email işleniyor: {JobId}", job.Id);

        await Task.Delay(3000); // paralelliği görmek için
        throw new Exception("DLQ test hatası");
    }
}