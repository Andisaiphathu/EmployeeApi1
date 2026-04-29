using EmployeeManagementSystem.Services; 
using EmployeeManagementSystem.Models.Dtos;
using EmployeeManagementSystem.BackgroundServices;

public class EmailBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEmailQueue _queue;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(
        IServiceScopeFactory scopeFactory,
        IEmailQueue queue, 
        ILogger<EmailBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
     {
      try
        {
           var item = await _queue.DequeueAsync(stoppingToken);
           
           using var scope = _scopeFactory.CreateScope();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

           await item.ProcessAsync(emailSender);

        }
    catch (Exception ex)
        {
            _logger.LogError(ex, "Email processing failed");
        }
     }
    }
}