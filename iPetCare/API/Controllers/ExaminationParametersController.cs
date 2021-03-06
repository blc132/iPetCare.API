﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Dtos.ExaminationParameters;
using Application.Services.Utilities;
using Microsoft.AspNetCore.Authorization;
using Domain.Models;
using API.Security;

namespace API.Controllers
{
    public class ExaminationParametersController : BaseController
    {
        private readonly IExaminationParameterService _examinationParameterService;
        public ExaminationParametersController(IExaminationParameterService examinationParameterService)
        {
            _examinationParameterService = examinationParameterService;
        }

        [Produces(typeof(ServiceResponse<CreateExaminationParameterDtoResponse>))]
        [Authorize(Roles = Role.Administrator)]
        [HttpPost]
        public async Task<IActionResult> CreateExaminationParameter(CreateExaminationParameterDtoRequest dto)
        {
            var response = await _examinationParameterService.CreateExaminationParameterAsync(dto);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse<GetAllExaminationParametersDtoResponse>))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpGet]
        public async Task<IActionResult> GetAllExaminationParameters()
        {
            var response = await _examinationParameterService.GetAllExaminationParametersAsync();
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse<GetExaminationParameterDtoResponse>))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpGet("{examinationParameterId}")]
        public async Task<IActionResult> GetExaminationParameter(int examinationParameterId)
        {
            var response = await _examinationParameterService.GetExaminationParameterAsync(examinationParameterId);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse<UpdateExaminationParameterDtoResponse>))]
        [Authorize(Roles = Role.Administrator)]
        [HttpPut("{examinationParameterId}")]
        public async Task<IActionResult> UpdateExaminationType(int examinationParameterId, UpdateExaminationParameterDtoRequest dto)
        {
            var response = await _examinationParameterService.UpdateExaminationParameterAsync(examinationParameterId, dto);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse))]
        [Authorize(Roles = Role.Administrator)]
        [HttpDelete("{examinationParameterId}")]
        public async Task<IActionResult> DeleteExaminationParameter(int examinationParameterId)
        {
            var response = await _examinationParameterService.DeleteExaminationParameterAsync(examinationParameterId);
            return SendResponse(response);
        }
    }
}
