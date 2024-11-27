using System.Security.Cryptography;
using System.Text;

namespace MapGeneration.API.Auth;

public static class Cryptography
{
    private const int Iterations = 10000;
    private const int KeySize = 64;

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(KeySize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );
        return Convert.ToHexString(hash);
    }

    public static bool VerifyPassword(string passwordHash, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(KeySize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );

        return CryptographicOperations.FixedTimeEquals(hash, Convert.FromHexString(passwordHash));
    }
}