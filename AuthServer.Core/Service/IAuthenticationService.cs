using AuthServer.Core.Dtos;
using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Service
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        // kullanıcının refreshToken'ı kaldırmak için veya kullanıcı logout olduğunda refresh token'ı silmek için kullanabiliriz
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);

        Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
    }
}
