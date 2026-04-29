using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Models.Dtos;
using EmployeeManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagementSystem.Controllers
{  //localhost:xxxx/api/employees
    [Route("api/employees")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase 
    {
        private readonly AppDbContext dbContext;
        public EmployeeController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        
        public async Task<IActionResult> GetAllEmployees()
        {
        var allEmployees = await dbContext.Employees.ToListAsync();
        return Ok(allEmployees);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id:guid}")]

        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
           var employee = await dbContext.Employees.FindAsync(id);
           if (employee is null)
           {
            return NotFound();
           }
           return Ok(employee);
        }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> AddEmployee(AddEmployeeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Department = dto.Department,
            Salary = dto.Salary,
            DateCreated = DateTime.Now
        };

        await dbContext.Employees.AddAsync(employee);
        await dbContext.SaveChangesAsync();

        return Ok(employee);
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPut("{id:guid}")]
            
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {

         if (!ModelState.IsValid)
        {
         return BadRequest(ModelState);
        }

        
        
        var employee = await dbContext.Employees.FindAsync(id);
        
        if (employee is null)
        {
            return NotFound();
        }

        employee.FirstName = updateEmployeeDto.FirstName;
        employee.LastName = updateEmployeeDto.LastName;
        employee.Email = updateEmployeeDto.Email;
        employee.Department = updateEmployeeDto.Department;
        employee.Salary = updateEmployeeDto.Salary;

        await dbContext.SaveChangesAsync();

        return Ok(employee);
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id:guid}")]

        public async Task<IActionResult> DeleteEmployee(Guid id)
        {

         
        var employee = await dbContext.Employees.FindAsync(id);

         if (employee is null)
         {
            return NotFound();
         }

         dbContext.Employees.Remove(employee);
         await dbContext.SaveChangesAsync();

         return NoContent();

         
        }
       
    }
}
 