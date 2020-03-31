﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Application.Interfaces;
using Application.Services.Utilities;
using Application.Dtos.ExaminationTypes;
using Domain.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services
{
    public class ExaminationTypeService : Service, IExaminationTypeService
    {
        public ExaminationTypeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>> CreateExaminationTypeAsync(ExaminationTypesCreateExaminationTypeDtoRequest dto)
        {
            if (await Context.ExaminationTypes.Where(x => x.Name == dto.Name).AnyAsync())
                return new ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>(HttpStatusCode.BadRequest, "Podane badanie już istnieje");

            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>(HttpStatusCode.Unauthorized);

            if (CurrentlyLoggedUser.Role != Role.Administrator)
                return new ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>(HttpStatusCode.Forbidden);

            var species = await Context.Species.FindAsync(dto.SpeciesId);
            if (species == null)
                return new ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>(HttpStatusCode.BadRequest, "Nie ma takiego gatunku");

            var examinationType = new ExaminationType()
            {
                Name = dto.Name,
                SpeciesId = dto.SpeciesId
            };

            Context.ExaminationTypes.Add(examinationType);
            int result = await Context.SaveChangesAsync();

            if (result > 0)
            {
                var responseDto = new ExaminationTypesCreateExaminationTypeDtoResponse()
                {
                    Name = examinationType.Name,
                    SpeciesId = examinationType.SpeciesId,
                    Id = examinationType.Id
                };

                return new ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>(HttpStatusCode.OK, responseDto);
            }

            return new ServiceResponse<ExaminationTypesCreateExaminationTypeDtoResponse>(HttpStatusCode.BadRequest);
        }

        public async Task<ServiceResponse<ExaminationTypesGetAllExaminationTypesDtoResponse>> GetAllExaminationTypesAsync()
        {
            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<ExaminationTypesGetAllExaminationTypesDtoResponse>(HttpStatusCode.Unauthorized);

            var examinationType = await Context.ExaminationTypes.ToListAsync();

            var dto = new ExaminationTypesGetAllExaminationTypesDtoResponse()
            {
                ExaminationTypes = Mapper.Map<List<ExaminationTypesDetailGetAllDtoResponse>>(examinationType)
            };

            return new ServiceResponse<ExaminationTypesGetAllExaminationTypesDtoResponse>(HttpStatusCode.OK, dto);
        }

        public async Task<ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>> UpdateExaminationTypeAsync(int raceId, ExaminationTypesUpdateExaminationTypeDtoRequest dto)
        {
            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.Unauthorized);

            if (CurrentlyLoggedUser.Role != Role.Administrator)
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.Forbidden);

            if (await Context.ExaminationTypes.Where(x => x.Name == dto.Name).AnyAsync())
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.BadRequest, "Podane badanie już istnieje");

            var examinationType = Context.ExaminationTypes.Find(raceId);
            var species = Context.Species.Find(dto.SpeciesId);

            if (species == null)
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.NotFound);

            if (examinationType == null)
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.NotFound);

            examinationType.Name = dto.Name;
            examinationType.SpeciesId = dto.SpeciesId;

            int result = await Context.SaveChangesAsync();
            if (result > 0)
            {
                var responseDto = Mapper.Map<ExaminationTypesUpdateExaminationTypeDtoResponse>(examinationType);
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.OK, responseDto);
            }

            if (result == 0)
                return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.BadRequest, "Nie nastąpiła żadna zmiana");

            return new ServiceResponse<ExaminationTypesUpdateExaminationTypeDtoResponse>(HttpStatusCode.BadRequest);
        }

        public async Task<ServiceResponse> DeleteExaminationTypeAsync(int examinationTypeId)
        {
            if (CurrentlyLoggedUser == null)
                return new ServiceResponse(HttpStatusCode.Unauthorized);

            if (CurrentlyLoggedUser.Role != Role.Administrator)
                return new ServiceResponse(HttpStatusCode.Forbidden);

            var examinationType = Context.ExaminationTypes.Find(examinationTypeId);
            if (examinationType == null)
                return new ServiceResponse(HttpStatusCode.NotFound);
            
            Context.ExaminationTypes.Remove(examinationType);
            int result = await Context.SaveChangesAsync();

            if (result > 0)
                return new ServiceResponse(HttpStatusCode.OK);

            return new ServiceResponse(HttpStatusCode.BadRequest);
        }

        public async Task<ServiceResponse<ExaminationParametersGetAllForOneExaminationTypeDtoResponse>> GetAllForOneExaminationTypeAsync(int examinationTypeId)
        {
            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<ExaminationParametersGetAllForOneExaminationTypeDtoResponse>(HttpStatusCode.Unauthorized);

            var examinationType = await Context.ExaminationTypes.FindAsync(examinationTypeId);
            if (examinationType == null)
                return new ServiceResponse<ExaminationParametersGetAllForOneExaminationTypeDtoResponse>(HttpStatusCode.NotFound);

            var examinationParameter = await Context.ExaminationParameters.Where(x => x.ExaminationTypeId == examinationTypeId).ToListAsync();

            var dto = new ExaminationParametersGetAllForOneExaminationTypeDtoResponse()
            {
                Id = examinationType.Id,
                Name = examinationType.Name,
                ExaminationParameters = Mapper.Map<List<ExaminationParameterDetailsForExaminationTypeGetDtoResponse>>(examinationParameter)
            };

            return new ServiceResponse<ExaminationParametersGetAllForOneExaminationTypeDtoResponse>(HttpStatusCode.OK, dto);
        }
    }
}
