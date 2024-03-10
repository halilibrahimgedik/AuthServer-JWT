using AuthServer.Core.Dtos;
using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Service
{
    public interface IUserService
    {
        Task<Response<CreateUserDto>>
    }
}
