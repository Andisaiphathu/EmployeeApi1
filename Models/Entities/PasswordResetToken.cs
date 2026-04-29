
namespace EmployeeManagementSystem.Models.Entities
{
public class PasswordResetToken
{
    public int Id { get; set; }

    public string TokenId { get; set; } = ""; 
    public string TokenHash { get; set; } = "";

    public DateTime Expiry { get; set; }

    public int UserId { get; set; }

    public UserAccount User { get; set; } = null!;
    
}
}