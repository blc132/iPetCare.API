﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Dtos.Users;
using Application.Interfaces;
using Application.Services.Utilities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services
{
    public class UserService: IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtGenerator jwtGenerator, DataContext context, IUserAccessor userAccessor)
        {
            _jwtGenerator = jwtGenerator;
            _context = context;
            _userAccessor = userAccessor;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<ServiceResponse<LoginDtoResponse>> LoginAsync(LoginDtoRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return new ServiceResponse<LoginDtoResponse>(HttpStatusCode.Unauthorized);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (result.Succeeded)
            {
                var responseDto = new LoginDtoResponse()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _jwtGenerator.CreateToken(user),
                    Role = user.Role,
                };

                return new ServiceResponse<LoginDtoResponse>(HttpStatusCode.OK, responseDto);
            }

            return new ServiceResponse<LoginDtoResponse>(HttpStatusCode.Unauthorized);
        }

        public async Task<ServiceResponse<RegisterDtoResponse>> RegisterAsync(RegisterDtoRequest dto)
        {
            if (await _context.Users.Where(x => x.Email == dto.Email).AnyAsync())
                return new ServiceResponse<RegisterDtoResponse>(HttpStatusCode.BadRequest, "Email already exists");

            if (await _context.Users.Where(x => x.UserName == dto.UserName).AnyAsync())
                return new ServiceResponse<RegisterDtoResponse>(HttpStatusCode.BadRequest, "Nick already exists");

            if (dto.Role == Role.Administrator)
            {
                var currentUserName = _userAccessor.GetCurrentUsername();

                if (currentUserName == null)
                    return new ServiceResponse<RegisterDtoResponse>(HttpStatusCode.BadRequest, "No permissions to register an account with this role");

                var currentUser = await _userManager.FindByNameAsync(currentUserName);
                if (currentUser == null || currentUser.Role != Role.Administrator)
                    return new ServiceResponse<RegisterDtoResponse>(HttpStatusCode.BadRequest, "No permissions to register an account with this role");
            }

            if (dto.Role == Role.Owner || dto.Role == Role.Vet)
            {
                var user = new ApplicationUser()
                {
                    LastName = dto.LastName,
                    FirstName = dto.FirstName,
                    Email = dto.Email,
                    UserName = dto.UserName,
                    Role = dto.Role
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded)
                {
                    var responseDto = new RegisterDtoResponse()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.UserName,
                        Role = user.Role,
                        Token = _jwtGenerator.CreateToken(user)
                    };

                    return new ServiceResponse<RegisterDtoResponse>(HttpStatusCode.OK, responseDto);
                }
            }
            return new ServiceResponse<RegisterDtoResponse>(HttpStatusCode.Unauthorized);
        }
    }
}
