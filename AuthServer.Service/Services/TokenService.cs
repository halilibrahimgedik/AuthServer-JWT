using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;

        private readonly CustomTokenOption _tokenOption;
        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }


        private string CreateRefreshToken()
        {
            //return Guid.NewGuid().ToString();

            var numberByte = new Byte[32];
            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }

        private IEnumerable<Claim> GetClaim(UserApp user, List<String> audiences)
        {
            //payload'a yükleyeceğimiz kritik olmayan veri her biri veri claim olarak ifade edilir
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),

                // random guid değeri oluşturalım Token için - Best practise
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            userClaims.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            return userClaims;
        }


        private IEnumerable<Claim> GetClaimForClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            // random guid değeri oluşturalım Token için - Best practise
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

            // bu token kimin için oluşturuluyor 
            new Claim(JwtRegisteredClaimNames.Sub, client.Id);

            return claims;
        }

        public TokenDto CreateToken(UserApp user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.RefreshTokenExpiration);

            var securityKey = SignService.GetSymetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires : accessTokenExpiration,
                notBefore : DateTime.UtcNow,
                claims : GetClaim(user,_tokenOption.Audience),
                signingCredentials : signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration);

            var securityKey = SignService.GetSymetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                claims: GetClaimForClient(client),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };

            return tokenDto;
        }
    }
}
