using EmployeeManagementSystem.Models.Dtos; 
using System.Threading.Channels; 

namespace EmployeeManagementSystem.Services
 {

public class EmailQueue : IEmailQueue
{
    private readonly Channel<IEmailJob> _queue = Channel.CreateBounded<IEmailJob>(1000);

    public void Enqueue(IEmailJob job)
    {
        _queue.Writer.TryWrite(job);
    }

    public async Task<IEmailJob> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
 }
}