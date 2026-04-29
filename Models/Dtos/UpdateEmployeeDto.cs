using System.ComponentModel.DataAnnotations;


namespace EmployeeManagementSystem.Models.Dtos
{
    public class UpdateEmployeeDto
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Department { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }
    }
}