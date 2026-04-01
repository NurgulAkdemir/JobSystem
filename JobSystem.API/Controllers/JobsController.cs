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

    public JobsController(IJobQueue jobQueue)
    {
        _jobQueue = jobQueue;
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
}



