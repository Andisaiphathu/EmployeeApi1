namespace EmployeeManagementSystem.Models.Entities
{
    public class UserAccount
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }

    public List<PasswordResetToken> PasswordResetTokens { get; set; } = new();
    
}
}