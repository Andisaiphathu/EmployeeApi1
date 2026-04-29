using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Handlers;
using EmployeeManagementSystem.Models.Entities;
using System.Security.Claims;

namespace EmployeeManagementSystem.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly PasswordHashHandler _passwordHandler;

        public UserAccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHandler = new PasswordHashHandler();
        }
       
       [Authorize(Roles = "Admin,SuperAdmin")]
       [HttpGet]

       public async Task<IActionResult> GetUsers()
       {
        var users = await _dbContext.UserAccounts
        .Select(x => new
        {
            x.Id,
            x.FullName,
            x.EmailAddress,
            x.Role
        })
        .ToListAsync();

        return Ok(users);

        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)
        {
         var user = await _dbContext.UserAccounts
        .Where(x => x.Id == id)
        .Select(x => new
        {
            x.Id,
            x.FullName,
            x.EmailAddress,
            x.Role
        })
        .FirstOrDefaultAsync();

        if (user == null)
        return NotFound();

         return Ok(user);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto request)
        {
        if (!ModelState.IsValid)
        {
        return BadRequest(ModelState);
        }

        var user = await _dbContext.UserAccounts.FindAsync(id);

        if (user == null)
        {
        return NotFound();
        }

        user.FullName = request.FullName;
        user.EmailAddress = request.EmailAddress;

        var validRoles = new[] { "User", "Admin", "SuperAdmin" };

       if (!validRoles.Contains(request.Role))
       {
        return BadRequest("Invalid role");
       }

       var currentUserRole = User.FindFirst(ClaimTypes.Name)?.Value;

      if (user.Role == "SuperAdmin" && currentUserRole != "SuperAdmin")
      {
      return Forbid("Only SuperAdmin can modify SuperAdmin");
      }
        
      
       var currentUserEmail = User.FindFirst(ClaimTypes.Role)?.Value;

      if (currentUserEmail == null)
      {
         return Unauthorized();
      }

        if (user.EmailAddress == currentUserEmail && request.Role != user.Role)
        {
          return BadRequest("You cannot change your own role");
        }

        user.Role = request.Role;

        
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
        user.Password = _passwordHandler.HashPassword(request.Password);
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
        }
        

       [Authorize(Roles = "SuperAdmin")]
       [HttpPost("create-admin")]
       public async Task<IActionResult> CreateAdmin(CreateAdminDto request)
       {
         if (!ModelState.IsValid)
           return BadRequest(ModelState);

        var existingUser = await _dbContext.UserAccounts
        .FirstOrDefaultAsync(x => x.EmailAddress == request.EmailAddress);

         if (existingUser != null)
           return BadRequest("User already exists");

        var user = new UserAccount
       {
        FullName = request.FullName,
        EmailAddress = request.EmailAddress,
        Password = _passwordHandler.HashPassword(request.Password),
        Role = "Admin"
        };

       _dbContext.UserAccounts.Add(user);
       await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new
        {
          user.Id,
          user.FullName,
          user.EmailAddress,
          user.Role
        });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
         var user = await _dbContext.UserAccounts.FindAsync(id);

         if (user == null)
        {
        return NotFound();
        }

       var currentUserEmail = User.FindFirst(ClaimTypes.Name)?.Value;

       if (currentUserEmail == null)
       {
           return Unauthorized();
       }
        // Prevent deleting yourself
        if (user.EmailAddress == currentUserEmail)
        {
        return BadRequest("You cannot delete your own account");
        }

        if (user.Role == "SuperAdmin")
        {
        return Forbid("SuperAdmin cannot be deleted");
        }

        _dbContext.UserAccounts.Remove(user);
        await _dbContext.SaveChangesAsync();

        return NoContent();
        }
    }
}


