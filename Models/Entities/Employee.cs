namespace EmployeeManagementSystem.Models.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }
        
        public required string Department { get; set; }

        public decimal Salary { get; set; }

        public DateTime DateCreated { get; set; }
    }
}