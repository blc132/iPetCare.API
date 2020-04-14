﻿using System.Net;
using System.Threading.Tasks;
using API.Security;
using Application.Dtos.Owners;
using Application.Dtos.Users;
using Application.Dtos.Vets;
using Application.Interfaces;
using Application.Services.Utilities;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginDtoResponse>> Login(LoginDtoRequest dto)
        {
            var response = await _userService.LoginAsync(dto);

            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(response.ResponseContent);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized(response.Message);
            if (response.StatusCode == HttpStatusCode.Forbidden)
                return Forbid(response.Message);
            return BadRequest(response.Message);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterDtoResponse>> Register(RegisterDtoRequest dto)
        {
            var response =  await _userService.RegisterAsync(dto);

            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(response.ResponseContent);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized(response.Message);
            if (response.StatusCode == HttpStatusCode.Forbidden)
                return Forbid(response.Message);
            return BadRequest(response.Message);
        }

        [Authorize(Roles = Role.Administrator)]
        [HttpGet("")]
        public async Task<ActionResult<GetAllUsersDtoResponse>> GetUsers()
        {
            var response = await _userService.GetAllAsync();

            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(response.ResponseContent);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized(response.Message);
            if (response.StatusCode == HttpStatusCode.Forbidden)
                return Forbid(response.Message);
            return BadRequest(response.Message);
        }

        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpPut]
        public async Task<ActionResult<EditProfileDtoResponse>> EditProfile(EditProfileDtoRequest dto)
        {
            var response = await _userService.EditProfileAsync(dto);

            if (response.StatusCode == HttpStatusCode.OK)
                return Ok(response.ResponseContent);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized(response.Message);
            if (response.StatusCode == HttpStatusCode.Forbidden)
                return Forbid(response.Message);
            return BadRequest(response.Message);
        }

        [Produces(typeof(ServiceResponse<GetVetsDtoResponse>))]
        [Authorize]
        [HttpPost("vets")]
        public async Task<IActionResult> GetVets([FromBody] GetVetsDtoRequest dto)
        {
            var response = await _userService.GetVetsAsync(dto);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse<GetOwnersDtoResponse>))]
        [Authorize]
        [HttpPost("owners")]
        public async Task<IActionResult> GetOwners([FromBody] GetOwnersDtoRequest dto)
        {
            var response = await _userService.GetOwnersAsync(dto);
            return SendResponse(response);
        }
    }
}