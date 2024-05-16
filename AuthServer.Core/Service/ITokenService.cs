using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Service
{
    public interface ITokenService
    {
        Task<TokenDto> CreateTokenAsync(UserApp userApp);

        ClientTokenDto CreateTokenByClient(Client client);
    }
}
