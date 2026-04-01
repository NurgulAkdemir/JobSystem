using JobSystem.Domain.Entities;

namespace JobSystem.Application.Interfaces
{
    public interface IJobQueue
    {
        void Enqueue(Job job);
        bool TryDequeue(out Job job);
    }
}