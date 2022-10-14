using Abstract.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Concrete.Services
{
    public class SecurityService : Abstract.Services.ISecurityService
    {
        private readonly Abstract.Settings.JwtSettings _jwtSettings;

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 1000;

        public string IdClaim => "id";

        public SecurityService(IOptions<Abstract.Settings.JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateJwtToken(User user, TimeSpan validityPeriod)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(IdClaim, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.Add(validityPeriod),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Hash Hash(string plaintext)
        {
            using var algorithm = new Rfc2898DeriveBytes(plaintext, SaltSize, Iterations, HashAlgorithmName.SHA256);
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);
            return new Hash() { Salt = salt, Key = key };
        }

        public JwtSecurityToken? ValidateToken(string token, bool validateLifetime = true)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = validateLifetime
                }, out SecurityToken validatedToken);
                return (JwtSecurityToken)validatedToken;
            }
            catch
            {
                return null;
            }
        }

        public bool Verify(Hash hash, string plaintext)
        {
            byte[] salt = Convert.FromBase64String(hash.Salt);
            byte[] key = Convert.FromBase64String(hash.Key);

            using var algorithm = new Rfc2898DeriveBytes(plaintext, salt, Iterations, HashAlgorithmName.SHA256);
            var keyToCheck = algorithm.GetBytes(KeySize);
            return keyToCheck.SequenceEqual(key);
        }
    }
}
