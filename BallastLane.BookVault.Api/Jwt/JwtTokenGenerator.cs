using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BallastLane.BookVault.Api.Jwt
{
    public class JwtTokenGenerator(string jwtSecret, JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        private readonly string _jwtSecret = jwtSecret;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = jwtSecurityTokenHandler;

        public string GenerateToken(string userIdentifier)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "BallastLane",
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, userIdentifier)
                ]),

                Expires = DateTime.UtcNow.AddHours(36),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);

            return _jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
