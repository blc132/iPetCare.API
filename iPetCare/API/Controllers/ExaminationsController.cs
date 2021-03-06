﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Dtos.Examinations;
using Application.Services.Utilities;
using Domain.Models;
using API.Security;

namespace API.Controllers
{
    public class ExaminationsController : BaseController
    {

        private readonly IExaminationService _examinationsService;
        public ExaminationsController(IExaminationService examinationsService)
        {
            _examinationsService = examinationsService;
        }

        [Produces(typeof(ServiceResponse<CreateExaminationDtoResponse>))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpPost]
        public async Task<IActionResult> CreateExamination(CreateExaminationDtoRequest dto)
        {
            var response = await _examinationsService.CreateExaminationAsync(dto);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse<GetAllExaminationsDtoResponse>))]
        [AuthorizeRoles(Role.Administrator)]
        [HttpGet]
        public async Task<IActionResult> GetAllExaminations()
        {
            var response = await _examinationsService.GetAllExaminationsAsync();
            return SendResponse(response);
        }
        [Produces(typeof(ServiceResponse<GetAllExaminationsDtoResponse>))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetPetExaminations(Guid petId)
        {
            var response = await _examinationsService.GetPetExaminationsAsync(petId);
            return SendResponse(response);
        }
        [Produces(typeof(ServiceResponse<GetExaminationDtoResponse>))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpGet("{examinationId}")]
        public async Task<IActionResult> GetExamination(Guid examinationId)
        {
            var response = await _examinationsService.GetExaminationAsync(examinationId);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse<CreateExaminationDtoResponse>))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpPut("{petId}/{examinationId}")]
        public async Task<IActionResult> UpdateExamination(Guid petId, Guid examinationId, UpdateExaminationDtoRequest dto)
        {
            var response = await _examinationsService.UpdateExaminationAsync(petId, examinationId, dto);
            return SendResponse(response);
        }

        [Produces(typeof(ServiceResponse))]
        [AuthorizeRoles(Role.Administrator, Role.Vet, Role.Owner)]
        [HttpDelete("{petId}/{examinationId}")]
        public async Task<IActionResult> DeleteExamination(Guid petId, Guid examinationId)
        {
            var response = await _examinationsService.DeleteExaminationAsync(petId, examinationId);
            return SendResponse(response);
        }
    }
}
