namespace EmployeeManagementSystem.Services
{
    public interface IEmailJob
    {
        Task ProcessAsync(IEmailSender sender);
    }
}