using JobSystem.Domain.Entities;

namespace JobSystem.Application.Interfaces;

public interface IDeadLetterQueue
{
    void Enqueue(Job job);
    List <Job> GetAll();

}