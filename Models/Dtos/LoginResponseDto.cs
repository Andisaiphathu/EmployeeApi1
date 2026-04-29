namespace EmployeeManagementSystem.Models.Dtos
{
    public class LoginResponseDto
    {
        public required string Token { get; set;}
        public int UserId { get; set; }
        public required string Email { get; set; }= "";
        public required string Role { get; set; }
        public required string Message { get; set; }
        public required string RefreshToken { get; set; }
        public bool IsNewDevice { get; set; }
    }
}