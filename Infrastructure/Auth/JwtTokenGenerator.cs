using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Training.Angular.Api.Application.Common;
using Training.Angular.Api.Application.Interfaces;

namespace Training.Angular.Api.Infrastructure.Auth;

public sealed class JwtTokenGenerator(IOptions<JwtOptions> options) : ITokenGenerator
{
    public Task<GeneratedToken> GenerateAsync(string username, string role, CancellationToken ct)
    {
        var jwt = options.Value;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.Add(jwt.ValidFor);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role, role)
            },
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: signingCredentials);

        var encoded = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new GeneratedToken(encoded, expires));
    }
}
