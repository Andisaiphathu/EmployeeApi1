using EmployeeManagementSystem.Controllers;
using EmployeeManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Handlers;

namespace EmployeeManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var passwordHandler = new PasswordHashHandler();

        modelBuilder.Entity<Employee>()
        .Property(e => e.Salary)
        .HasPrecision(18, 2);
        
        modelBuilder.Entity<UserAccount>().HasData(
    
        new UserAccount
        {
        Id = 1, 
        FullName = "Super Admin",
        EmailAddress = "superadmin@example.com",
        Password = passwordHandler.HashPassword("ChangeMe123!"),
        Role = "SuperAdmin"
        }
        );

        var resetToken = modelBuilder.Entity<PasswordResetToken>();

        resetToken.HasIndex(x => x.TokenId)
        .IsUnique();
        resetToken.HasIndex(x => x.UserId);
        resetToken.HasIndex(x => x.Expiry);

        resetToken.Property(x => x.TokenId)
        .HasMaxLength(500);

        resetToken.HasOne(x => x.User)
        .WithMany(u => u.PasswordResetTokens)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);
        } 
    }
}