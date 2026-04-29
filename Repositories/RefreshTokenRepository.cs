using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token);
        }

    public async Task DeleteAll(int userId)
    {
     var tokens = await _dbContext.RefreshTokens
        .Where(x => x.UserId == userId)
        .ToListAsync();

     _dbContext.RefreshTokens.RemoveRange(tokens);
    await _dbContext.SaveChangesAsync();
    }
    }
}