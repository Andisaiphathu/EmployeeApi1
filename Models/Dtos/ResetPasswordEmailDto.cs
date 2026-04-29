using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Models.Dtos
{
   public class ResetPasswordEmailDto : IEmailJob
{
    public string Email { get; set; } = "";
    public string Link { get; set; } = "";

    public async Task ProcessAsync(IEmailSender sender)
    {
        await sender.SendPasswordResetAsync(Email, Link);
    }
}
}