using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;

namespace JobSystem.Infrastructure.Queue;

public class InMemoryDeadLetterQueue : IDeadLetterQueue
{
    private readonly List<Job> _deadJobs = new();

    public void Enqueue(Job job)
    {
        _deadJobs.Add(job);

    }
    public List<Job> GetAll()
    {
        return _deadJobs;
    }
}