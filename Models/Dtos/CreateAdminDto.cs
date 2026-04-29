using System.ComponentModel.DataAnnotations;

public class CreateAdminDto
{
    [Required]
    public required string FullName { get; set; }

    [Required]
    public required string EmailAddress { get; set; }

    [Required]
    public required string Password { get; set; }
}