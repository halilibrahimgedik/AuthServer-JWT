﻿using AuthServer.Core.Dtos;
using AuthServer.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Apı.Controllers
{
    [Authorize(AuthenticationSchemes = ("Bearer"), Roles = "admin")] // !!!
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : CustomBaseController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {

            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            
            return ActionResultInstance(users);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return ActionResultInstance(await _userService.GetUserByUserNameAsync(HttpContext.User.Identity.Name));
        }

        [HttpPost("[action]/{roleName}")]
        public async Task<IActionResult> CreateRole(string? roleName)
        {
            return ActionResultInstance(await _userService.CreateRoleAsync(roleName!));
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(AssignRoleToUserDto dto)
        {
            return ActionResultInstance(await _userService.AssignRoleToUserAsync(dto));
        }
    }
}
