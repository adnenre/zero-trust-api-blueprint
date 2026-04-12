using BCryptNet = BCrypt.Net.BCrypt;

namespace ZeroTrustAPI.Api.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCryptNet.HashPassword(password);
    public bool Verify(string password, string hash) => BCryptNet.Verify(password, hash);
}