using EmployeeManagementSystem.Models.Dtos;

namespace EmployeeManagementSystem.Services
{
    public interface IEmailSender
    {
        Task SendLoginAlertAsync(LoginAlertEmailDto dto);
        Task SendPasswordResetAsync(string email, string link);
    }
}