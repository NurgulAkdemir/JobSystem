using Microsoft.AspNetCore.Mvc;
using JobSystem.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IJobMetricsService _metrics;
    private readonly IDeadLetterQueue _deadLetterQueue;

    public DashboardController(
        IJobMetricsService metrics,
        IDeadLetterQueue deadLetterQueue)
    {
        _metrics = metrics;
        _deadLetterQueue = deadLetterQueue;
    }

    [HttpGet]
    public ContentResult GetDashboard()
    {
        var stats = _metrics.GetMetrics();
        var deadCount = _deadLetterQueue.GetAll().Count;

        var html = $@"
        <html>
        <head>
            <title>Job System Dashboard</title>
            <style>
                body {{ font-family: Arial; padding: 40px; background:#f4f4f4; }}
                .card {{ background:white; padding:20px; margin:10px; border-radius:10px; box-shadow:0 2px 5px rgba(0,0,0,0.1); }}
                h1 {{ color:#333; }}
            </style>
        </head>
        <body>
            <h1>🚀 Job System Dashboard</h1>

            <div class='card'>Total Jobs: {stats.TotalJobs}</div>
            <div class='card'>Success Jobs: {stats.SuccessJobs}</div>
            <div class='card'>Failed Jobs: {stats.FailedJobs}</div>
            <div class='card'>Retry Count: {stats.RetriedJobs}</div>
            <div class='card'>DLQ Count: {deadCount}</div>
        </body>
        </html>";

        return Content(html, "text/html");
    }
}