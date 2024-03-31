using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;
using Microsoft.IdentityModel.Tokens;

namespace ImageBrowser.Infrastructure.Services;
public class TokenFactory : ITokenFactory
{
    public TokenResult TokenGenerator(string secretToken, string secreturl, string userName, List<string> roles, string IdentityUSerId, int AppUserId, int expireMiniutes, int accessTokenExpireMinutes)
    {
        var expiresIn = DateTime.UtcNow.AddMinutes(accessTokenExpireMinutes);
        var expireSeconds = expiresIn.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, IdentityUSerId),
                new Claim("UserId", AppUserId.ToString())
            };

        // Add roles to claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretToken));
        var signinCredentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);
        var tokenOption = new JwtSecurityToken(
            issuer: secreturl,
            claims: claims,
            expires: expiresIn,
            signingCredentials: signinCredentials

            );
        var refreshTokenOption = new JwtSecurityToken(
            issuer: secreturl,
            expires: DateTime.UtcNow.AddMinutes(expireMiniutes),
            signingCredentials: signinCredentials

            );


        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOption);
        var refToken = new JwtSecurityTokenHandler().WriteToken(refreshTokenOption);
        return new TokenResult()
        {
            Token = tokenString,
            RefreshToken = refToken,
            ExpiresIn = expireSeconds
        };
    }


}
