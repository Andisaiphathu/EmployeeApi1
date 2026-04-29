using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementSystem.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.BackgroundServices
{
    public class ExpiredTokenRemovalBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredTokenRemovalBackgroundService> _logger;

        public ExpiredTokenRemovalBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ExpiredTokenRemovalBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

    while (!stoppingToken.IsCancellationRequested)
        {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expiredTokens = await dbContext.RefreshTokens
                .Where(t => t.Expiry < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                dbContext.RefreshTokens.RemoveRange(expiredTokens);
                await dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Expired tokens cleaned at: {time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error cleaning expired tokens: {message}", ex.Message);
        }

        await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
  }
}