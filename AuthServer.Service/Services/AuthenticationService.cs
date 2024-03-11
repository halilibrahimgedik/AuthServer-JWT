using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.IUnitOfWork;
using AuthServer.Core.Repositories;
using AuthServer.Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshToken;
        public AuthenticationService(IOptions<List<Client>> optionsClient,ITokenService tokenService,UserManager<UserApp> userManager,IUnitOfWork unitOfWork,IGenericRepository<UserRefreshToken> userRefreshToken )
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshToken = userRefreshToken;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if(loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Response<TokenDto>.Fail("Email or Password is wrong", 400,true);

            if(!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshToken.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if(userRefreshToken == null)
            {
                await _userRefreshToken.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    RefreshToken = token.RefreshToken,
                    Expiration = token.RefreshTokenExpiration,
                });
            }
            else
            {
                //refresh token güncellenecek
                userRefreshToken.RefreshToken = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.SaveChangesAsync();

            return Response<TokenDto>.Success(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x=>x.Id == clientLoginDto.Id && x.Secret == clientLoginDto.ClientSecret);

            if (client == null) return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found",404,true);

            var token = _tokenService.CreateTokenByClient(client);

            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshToken.Where(x => x.RefreshToken == refreshToken && x.Expiration != DateTime.UtcNow).SingleOrDefaultAsync();

            if(existRefreshToken == null)
            {
                return Response<TokenDto>.Fail("refresh token not found", 404, true);
            }

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if(user == null) return Response<TokenDto>.Fail("UserId not found", 404, true);

            var token = _tokenService.CreateToken(user);

            // eski refresh token bilgilerini yenisi ile güncelleyelim
            existRefreshToken.RefreshToken = token.RefreshToken;
            existRefreshToken.Expiration = token.RefreshTokenExpiration;

            await _unitOfWork.SaveChangesAsync();

            return Response<TokenDto>.Success(token,200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshToken.Where(x => x.RefreshToken == refreshToken && x.Expiration != DateTime.UtcNow).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("refresh token not found", 404, true);
            }

            _userRefreshToken.Remove(existRefreshToken);

            await _unitOfWork.SaveChangesAsync();

            return Response<NoDataDto>.Success(200);
        }
    }
}
