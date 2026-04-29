using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string EmailAddress { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}