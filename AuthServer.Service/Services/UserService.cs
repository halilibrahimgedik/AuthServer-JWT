using AuthServer.Core.Dtos;
using AuthServer.Core.Entities;
using AuthServer.Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dto;
using SharedLibrary.Exceptions;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return ResponseDto<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }

            return ResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<ResponseDto<UserAppDto>> GetUserByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null) return ResponseDto<UserAppDto>.Fail("UserName not found", 404, true);

            return ResponseDto<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }


        public async Task<ResponseDto<NoDataDto>> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return ResponseDto<NoDataDto>.Fail($"Invalid RoleName.", StatusCodes.Status400BadRequest, true);
            }

            var result = await _roleManager.CreateAsync(new() { Name = roleName });

            if (!result.Succeeded)
            {
                throw new RoleCreationException("Error Occured While Creating a Role");
            }

            return ResponseDto<NoDataDto>.Success(StatusCodes.Status201Created);
        }

        public async Task<ResponseDto<NoDataDto>> AssignRoleToUser(AssignRoleToUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                throw new UserNotFoundException($"There is no an user with id: {dto.UserId}");
            }

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!result.Succeeded)
            {
                throw new AssigningRoleException($"Error Occured While Assignin a Role to {user}");
            }

            return ResponseDto<NoDataDto>.Success(StatusCodes.Status201Created);
        }

    }
}
