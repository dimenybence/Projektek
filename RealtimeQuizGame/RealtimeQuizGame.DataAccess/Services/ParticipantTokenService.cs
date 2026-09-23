using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealtimeQuizGame.DataAccess.Config;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Services
{
    internal class ParticipantTokenService : IParticipantTokenService
    {
        private readonly JwtSettings _settings;

        public ParticipantTokenService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string CreateToken(Guid participationId, int quizId)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, participationId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(AuthClaimTypes.ParticipationId, participationId.ToString()),
            new(AuthClaimTypes.QuizId, quizId.ToString()),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.ParticipantAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ParticipantTokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
