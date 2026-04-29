using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
    public int Id { get; set; }

    [Required]
    public required string FullName { get; set; }

    [Required]
    [EmailAddress]
    public required string EmailAddress { get; set; }
    
    
    public string? Password { get; set; }

    [Required]
    public required string Role { get; set; }
}