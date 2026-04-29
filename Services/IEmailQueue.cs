namespace EmployeeManagementSystem.Services
{
    public interface IEmailQueue
    {
        void Enqueue(IEmailJob job);
        Task<IEmailJob> DequeueAsync(CancellationToken cancellationToken);
    }
}