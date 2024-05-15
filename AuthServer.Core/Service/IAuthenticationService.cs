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
        Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        Task<ResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        // kullanıcının refreshToken'ı kaldırmak için veya kullanıcı logout olduğunda refresh token'ı silmek için kullanabiliriz
        Task<ResponseDto<NoDataDto>> RevokeRefreshToken(string refreshToken);

        ResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
    }
}
