using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Models.Dtos
{
public class LoginAlertEmailDto : IEmailJob
{
    public async Task ProcessAsync(IEmailSender sender)
    {
        await sender.SendLoginAlertAsync(this);
    }    public required string ToEmail { get; set; }
    public required string Device { get; set; }
    public required string Ip { get; set; }
    public DateTime Time { get; set; }
}
}