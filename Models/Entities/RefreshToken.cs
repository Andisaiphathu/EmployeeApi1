namespace EmployeeManagementSystem.Models.Entities
{

public class RefreshToken
{
    public int Id { get; set; }
    public required string Token { get; set; }

    public required string TokenId { get; set; }
    public DateTime Expiry { get; set; }
    public int UserId { get; set; }

    public required string DeviceName { get; set; }

    public string? IpAddress { get; set; }

}
}