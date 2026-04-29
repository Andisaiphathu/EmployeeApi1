using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Security;
using EmployeeManagementSystem.Models.Dtos;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models.Entities;
using EmployeeManagementSystem.Handlers;
using EmployeeManagementSystem.Repositories;
using System.Security.Claims;
using EmployeeManagementSystem.Services;
using Microsoft.Extensions.Options;
using EmployeeManagementSystem.Settings;

namespace EmployeeManagementSystem.Controllers
{
[Route("api/auth")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly AppDbContext _dbContext;
    private readonly RefreshTokenRepository _refreshTokenRepository;

    private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;
    
    private readonly IEmailQueue _emailQueue;

    private readonly IConfiguration _configuration;

    private readonly AppSettings _appSettings;


    public AccountController(
        JwtService jwtService,
        AppDbContext dbContext,
        RefreshTokenRepository refreshTokenRepository,
         IEmailSender emailSender, IEmailQueue emailQueue, 
         IConfiguration configuration , 
         ILogger<AccountController> logger,
         IOptions<AppSettings> appSettings)
    {
        _jwtService = jwtService;
        _dbContext = dbContext;
        _refreshTokenRepository = refreshTokenRepository;
        _emailSender = emailSender;
        _emailQueue = emailQueue;
        _logger = logger;
        _configuration = configuration;
        _appSettings = appSettings.Value;

    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var passwordHandler = new PasswordHashHandler();

        var existingUser = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.EmailAddress == request.EmailAddress);

        if (existingUser != null)
            throw new ApplicationException("User already exists");

        var user = new UserAccount
        {
            FullName = request.FullName,
            EmailAddress = request.EmailAddress,
            Password = passwordHandler.HashPassword(request.Password),
            Role = "User"
        };

        _dbContext.UserAccounts.Add(user);
        await _dbContext.SaveChangesAsync();

        return Created("", new { message = "User registered successfully" });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(RequestForgotPasswordDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
                // validate user
                var user = await _dbContext.UserAccounts
                        .FirstOrDefaultAsync(x => x.EmailAddress == request.Email);
                
                if(user == null)
                   return Ok(new { message = "If the email exists, a reset link has been sent" });

                var passwordHandler = new PasswordHashHandler();

                var tokenId = Guid.NewGuid().ToString();
                var rawToken = Guid.NewGuid().ToString();

                var hashedToken = passwordHandler.HashPassword(rawToken);

                var oldTokens = await _dbContext.PasswordResetTokens
                 .Where(x => x.UserId == user.Id)
                 .ToListAsync();

                _dbContext.PasswordResetTokens.RemoveRange(oldTokens);

    
                var resetToken = new PasswordResetToken
                
                {
                 TokenId = tokenId,
                 TokenHash = hashedToken,
                 UserId = user.Id,
                 Expiry = DateTime.UtcNow.AddMinutes(15)
                };
                
                _dbContext.PasswordResetTokens.Add(resetToken);

                 await _dbContext.SaveChangesAsync();

                var callbackUrl = $"{_appSettings.FrontendUrl}/reset-password?tokenId={tokenId}&token={rawToken}";
              
                // Send email
                _emailQueue.Enqueue(new ResetPasswordEmailDto
                {     
                  Email = user.EmailAddress,
                  Link = callbackUrl
                });

                return Ok(new { message = "Password reset link has been sent" });
        }

    [HttpPost("reset-password")]
    [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            if(!ModelState.IsValid)
            return BadRequest("Invalid payload");

            var storedToken = await _dbContext.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenId == request.TokenId);

            if (storedToken == null)
                 throw new UnauthorizedAccessException("Invalid token");

            var passwordHandler = new PasswordHashHandler();

            if (!passwordHandler.VerifyPassword(request.Token, storedToken.TokenHash))
                 throw new UnauthorizedAccessException("Invalid token");


            if (storedToken.Expiry < DateTime.UtcNow)
                 throw new UnauthorizedAccessException("Token expired");

            
            if (request.Password.Length < 8)
               return BadRequest("Password must be at least 8 characters");

            var user = storedToken.User;

            user.Password = passwordHandler.HashPassword(request.Password);

            // Remove used token
            _dbContext.PasswordResetTokens.Remove(storedToken);

            var refreshTokens = _dbContext.RefreshTokens
               .Where(x => x.UserId == user.Id);

            _dbContext.RefreshTokens.RemoveRange(refreshTokens);

            await _dbContext.SaveChangesAsync();

             return Ok("Password reset successful");
         }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

     if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
     {
       ipAddress = "127.0.0.1 (Localhost)";
     }

    var deviceName = Request.Headers["User-Agent"].ToString();

    if (string.IsNullOrEmpty(deviceName))
    {
      deviceName = "Unknown Device";
    }
      else if (deviceName.Length > 100)
    {
      deviceName = deviceName.Substring(0, 100);
    }

    _logger.LogInformation("Login attempt for {Email}", request.Email);

    var result = await _jwtService.Authenticate(request, deviceName, ipAddress);

    if (result == null)
    {
        _logger.LogWarning("Failed login attempt for {Email}", request.Email);
        return Unauthorized("Invalid email or password");
    }

    _logger.LogInformation("Login successful for {Email} from {IP} using {Device}", request.Email, ipAddress, deviceName);

      // SEND EMAIL AFTER SUCCESS LOGIN
     if (result.IsNewDevice)
    {
      var emailDto = new LoginAlertEmailDto
    {
        ToEmail = result.Email,
        Device = deviceName,
        Ip = ipAddress,
        Time = DateTime.UtcNow
    };
    
    _emailQueue.Enqueue(emailDto);

     _logger.LogInformation("New device detected → alert email sent to {Email}", result.Email);
    }
  return Ok(result);
}




    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
   {
    var email = User.FindFirstValue(ClaimTypes.Name);

    var user = await _dbContext.UserAccounts
        .FirstOrDefaultAsync(x => x.EmailAddress == email);

    if (user == null)
    return Unauthorized();

    var sessions = await _dbContext.RefreshTokens
        .Where(x => x.UserId == user.Id)
        .Select(x => new
        {
            x.DeviceName,
            x.IpAddress,
            x.Expiry
        })
        .ToListAsync();

    return Ok(sessions);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(RefreshRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token is required");

    var parts = request.RefreshToken.Split('.');
    if (parts.Length != 2)
    throw new UnauthorizedAccessException("Invalid token format");

    var tokenId = parts[0];
    var rawToken = parts[1];

    var storedToken = await _dbContext.RefreshTokens
    .FirstOrDefaultAsync(x => x.TokenId == tokenId);
    
    if (storedToken == null)
    throw new UnauthorizedAccessException("Invalid refresh token");

    var passwordHandler = new PasswordHashHandler();

    if (!passwordHandler.VerifyPassword(rawToken, storedToken.Token))
    throw new UnauthorizedAccessException("Invalid refresh token");

    if (storedToken.Expiry < DateTime.UtcNow)
    throw new UnauthorizedAccessException("Refresh token expired");

        var user = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id == storedToken.UserId);

        if (user == null)
            return Unauthorized();

        // delete old token
        _dbContext.RefreshTokens.Remove(storedToken);
        await _dbContext.SaveChangesAsync();

        var deviceName = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var newAccessToken = _jwtService.GenerateAccessTokenForUser(user.EmailAddress, user.Role, user.Id);
        var newRefreshToken = await _jwtService.GenerateRefreshToken(user.Id, deviceName, ipAddress);

        return Ok(new
        {
            token = newAccessToken,
            refreshToken = newRefreshToken
        });
    }

    [HttpPost("logout-others")]
    public async Task<IActionResult> LogoutOtherDevices(RefreshRequestDto request)
    {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
        return BadRequest("Refresh token is required");

    var parts = request.RefreshToken.Split('.');
    if (parts.Length != 2)
        return Unauthorized("Invalid token format");

    var tokenId = parts[0];
    var rawToken = parts[1];

    var storedToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(x => x.TokenId == tokenId);

    if (storedToken == null)
        return Unauthorized("Invalid refresh token");

    var passwordHandler = new PasswordHashHandler();

    if (!passwordHandler.VerifyPassword(rawToken, storedToken.Token))
        return Unauthorized("Invalid refresh token");

    var tokensToRemove = await _dbContext.RefreshTokens
        .Where(x => x.UserId == storedToken.UserId && x.Id != storedToken.Id)
        .ToListAsync();

    _dbContext.RefreshTokens.RemoveRange(tokensToRemove);
    await _dbContext.SaveChangesAsync();

    return Ok(new { message = "All other sessions revoked successfully" });
    }

    [HttpPost("logout-current")]
    public async Task<IActionResult> LogoutCurrent(RefreshRequestDto request)
    {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
        return BadRequest("Refresh token is required");

    var parts = request.RefreshToken.Split('.');
    if (parts.Length != 2)
        return Unauthorized("Invalid token format");

    var tokenId = parts[0];
    var rawToken = parts[1];

    var storedToken = await _dbContext.RefreshTokens
        .FirstOrDefaultAsync(x => x.TokenId == tokenId);

    if (storedToken == null)
        return Unauthorized("Invalid refresh token");

    var passwordHandler = new PasswordHashHandler();

    if (!passwordHandler.VerifyPassword(rawToken, storedToken.Token))
        return Unauthorized("Invalid refresh token");

    _dbContext.RefreshTokens.Remove(storedToken);
    await _dbContext.SaveChangesAsync();

    return NoContent();
    }

    [HttpDelete("logout")]
    public async Task<IActionResult> Logout()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);

        if (email == null)
            return Unauthorized();

        var user = await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.EmailAddress == email);

        if (user == null)
            return Unauthorized();

        await _refreshTokenRepository.DeleteAll(user.Id);
        return NoContent();
    }
  }
}