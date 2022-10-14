using System.IdentityModel.Tokens.Jwt;

namespace Abstract.Services
{
    public interface ISecurityService
    {
        Models.Hash Hash(string plaintext);

        bool Verify(Models.Hash hash, string plaintext);

        JwtSecurityToken? ValidateToken(string token, bool validateLifetime = true);

        string GenerateJwtToken(Models.User user, TimeSpan validityPeriod);

        string IdClaim { get; }
    }
}
