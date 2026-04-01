using JobSystem.Application.Interfaces;
using JobSystem.Domain.Entities;
using System.Collections.Concurrent;

namespace JobSystem.Infrastructure.Queue
{
    public class InMemoryJobQueue : IJobQueue
    {
        private readonly ConcurrentQueue<Job> _queue = new();

        public void Enqueue(Job job)
        {
            _queue.Enqueue(job);
        }

        public bool TryDequeue(out Job job)
        {
            return _queue.TryDequeue(out job);
        }
    }
}