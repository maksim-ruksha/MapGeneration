using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MapGeneration.BLL.Models.Users;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UserRole = MapGeneration.BLL.Models.Users.UserRole;

namespace MapGeneration.BLL.Services.Auth;

public class AuthService : IAuthService
{
    private const int KeySize = 256;
    private const int SaltSize = 128;
    private const int Iterations = 10000;

    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    private readonly IService<UserModel, UserEntity> _userService;

    public AuthService(
        IPasswordHasher<UserModel> passwordHasher,
        ILogger<AuthService> logger,
        IService<UserModel, UserEntity> userService)
    {
        _passwordHasher = passwordHasher;
        _logger = logger;
        _userService = userService;
    }

    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );
        byte[] hashBytes = new byte[KeySize + SaltSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);
        return Convert.ToHexString(hashBytes);
    }

    public string EncodeToken(UserModel userModel)
    {
        DateTime now = DateTime.UtcNow;
        ClaimsIdentity claimsIdentity = GetClaims(userModel);
        JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: Jwt.Issuer,
            audience: Jwt.Audience,
            notBefore: now,
            claims: claimsIdentity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(Jwt.Lifetime)),
            signingCredentials: new SigningCredentials(Jwt.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        string result = new JwtSecurityTokenHandler().WriteToken(jwt);
        return result;
    }

    public async Task<bool> VerifyToken(string token, UserModel userModel)
    {
        try
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = Jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = Jwt.GetSymmetricSecurityKey(),
                ValidateLifetime = true
            };

            JwtSecurityToken jwt;
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            jwt = (JwtSecurityToken)validatedToken;

            bool isIdValid = userModel.Id == Guid.Parse(jwt.Claims.First(claim => claim.Type == "Id").Value);
            UserRole jwtUserRole;
            Enum.TryParse(jwt.Claims.First(claim => claim.Type == ClaimTypes.Role).Value, out jwtUserRole);
            bool isRoleValid = userModel.Role == jwtUserRole;
            
            return isIdValid && isRoleValid;
        }
        catch (SecurityTokenValidationException e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        byte[] passwordHashBytes = Convert.FromHexString(passwordHash);
        byte[] salt = new byte[SaltSize];
        Array.Copy(passwordHashBytes, 0, salt, 0, SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );
        byte[] passwordHashExtracted = new byte[KeySize];
        Array.Copy(passwordHashBytes, SaltSize, passwordHashExtracted, 0, KeySize);
        return CryptographicOperations.FixedTimeEquals(passwordHashExtracted, hash);
    }

    private ClaimsIdentity GetClaims(UserModel userModel)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim("Id", userModel.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userModel.Name));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userModel.Role.ToString()));
        return claimsIdentity;
    }
}