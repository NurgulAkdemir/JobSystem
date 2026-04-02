using JobSystem.Domain.Entities;

public interface IJobHandler
{
    string JobType { get; }
    Task HandleAsync(Job job);
}