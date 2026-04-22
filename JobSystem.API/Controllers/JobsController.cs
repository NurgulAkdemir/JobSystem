using Microsoft.AspNetCore.Mvc;
using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;

namespace JobSystem.API.Controllers;

[ApiController]
[Route("api/jobs")]
[Route("api/controller")]

public class JobsController : ControllerBase
{
    [HttpGet("test")]
     public IActionResult Test()
    {
        return Ok ("Job System is Running ");
    }

     private readonly IJobQueue _jobQueue;
     private readonly IDeadLetterQueue _deadLetterQueue;
     private readonly IJobMetricsService _metrics;

    public JobsController(IJobQueue jobQueue, IDeadLetterQueue deadLetterQueue, IJobMetricsService metrics)
    {
        _jobQueue = jobQueue;
        _deadLetterQueue = deadLetterQueue;
        _metrics = metrics;
    }

    [HttpPost]
    public IActionResult CreateJob()
    {
        var job = new Job
        {
            Type = "Email",
            Payload = "Test payload"
        };

        _jobQueue.Enqueue(job);

        return Ok(job.Id);
    }
    
    [HttpGet("dead")]
    public IActionResult GetDeadJobs()
    {
        var jobs = _deadLetterQueue.GetAll();
        return Ok(jobs);
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        var stats = _metrics.GetMetrics();
        return Ok(stats);
    }
}



