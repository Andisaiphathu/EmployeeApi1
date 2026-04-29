using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Handlers;
using System.Text;


namespace EmployeeManagementSystem.Security
{
    public class JwtService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        private readonly RefreshTokenRepository _refreshTokenRepository;

        public JwtService(AppDbContext dbContext, 
        IConfiguration configuration, 
        RefreshTokenRepository refreshTokenRepository)       
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _refreshTokenRepository =  refreshTokenRepository;
        }

       public async Task<LoginResponseDto?> Authenticate(
        LoginRequestDto request, 
        string deviceName, 
        string ipAddress)

       {
        if (string.IsNullOrWhiteSpace(request.Email) || 
        string.IsNullOrWhiteSpace(request.Password))
       {
        return null;
       }
       
        var userAccount = await _dbContext.UserAccounts
        .FirstOrDefaultAsync(x => x.EmailAddress == request.Email);

        if (userAccount == null)
         return null;

        var passwordHandler = new PasswordHashHandler();

        if (!passwordHandler.VerifyPassword(request.Password, userAccount.Password))
         return null;

        var isKnownDevice = await _dbContext.RefreshTokens
        .AnyAsync(x => x.UserId == userAccount.Id &&
                       x.DeviceName == deviceName);

        var response = await GenerateTokenForUser(userAccount, deviceName, ipAddress);

        if (response == null)
        return null;

        response.IsNewDevice = !isKnownDevice;
        
        return response;
        }

        public string GenerateAccessTokenForUser(string email, string role, int userId)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");

            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            // Create claims
        var claims = new[]
        { 
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

            // Create token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
        
        public async Task<LoginResponseDto> GenerateTokenForUser(UserAccount user, 
        string deviceName, 
        string ipAddress)
        {
          var accessToken = GenerateAccessTokenForUser(user.EmailAddress, user.Role, user.Id);
         var refreshToken = await GenerateRefreshToken(user.Id, deviceName, ipAddress);

         return new LoginResponseDto
        {
          Token = accessToken,
          RefreshToken = refreshToken,
          Email = user.EmailAddress,
          UserId = user.Id,
          Role = user.Role,
          Message = "Login successful"
        };
        }
        public async Task<string> GenerateRefreshToken(int userAccountId, string deviceName, string ipAddress)

        {
        var refreshTokenValidityMins = _configuration
        .GetValue<int>("JwtConfig:RefreshTokenValidityMins");

        var passwordHandler = new PasswordHashHandler();

        var tokenId = Guid.NewGuid().ToString();
        //Generate RAW token (this goes to user)
        var rawToken = Guid.NewGuid().ToString();

        // HASHED token (stored in DB)
        var hashedToken = passwordHandler.HashPassword(rawToken);

        var refreshToken = new RefreshToken
       {

        TokenId = tokenId,
        Token = hashedToken, // store HASHED version
        Expiry = DateTime.UtcNow.AddMinutes(refreshTokenValidityMins),
        UserId =  userAccountId,
        DeviceName = deviceName,
        IpAddress = ipAddress
       };
       
        await _refreshTokenRepository.AddAsync(refreshToken);

        //Return COMBINED token to user
       return $"{tokenId}.{rawToken}";
       }
    }
}