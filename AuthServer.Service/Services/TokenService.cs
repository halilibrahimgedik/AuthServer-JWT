using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
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

        private readonly CustomTokenOptions _tokenOption;
        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options)
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

        private async Task<IEnumerable<Claim>> GetClaims(UserApp user, List<String> audiences)
        {
            //payload'a yükleyeceğimiz kritik olmayan veri her biri veri claim olarak ifade edilir
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),

                // random guid değeri oluşturalım Token için - Best practise
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),


            };

            userClaims.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            // 1-) *** Role-Based Authorization *** için role bilgisini claims'lere ekleyelim (payload'a eklenecek)
            // Rolleri alalım
            var userRoles = await _userManager.GetRolesAsync(user);
            // userRoles listesindeki rolleri tek tek dolaşıp claimsListesine ekledik
            userClaims.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));


            // 2-) *** Claim-Based Authorization ***
            /*new Claim("city",user.City) nullReferanceException almamak için user.City gini nullable bir alanın
                                           kesinlikle dolu olduğundan emin olduktan sonra claim oluşturulmalıdır*/

            if (!string.IsNullOrEmpty(user.City))
            {
                userClaims.Add(new Claim("city", user.City));
            }

            return userClaims;
        }


        private IEnumerable<Claim> GetClaimsForClient(Client client)
        {
            var claims = new List<Claim>()
            {
                // random guid değeri oluşturalım Token için - Best practise
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // bu token kimin için oluşturuluyor 
                new Claim(JwtRegisteredClaimNames.Sub, client.Id),
            };

            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            return claims;
        }

        public async Task<TokenDto> CreateTokenAsync(UserApp user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.RefreshTokenExpiration);

            var securityKey = SignService.GetSymetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                claims: await GetClaims(user, _tokenOption.Audience),
                signingCredentials: signingCredentials
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
                claims: GetClaimsForClient(client),
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
