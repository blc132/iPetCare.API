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
    public class ExaminationTypesService : Service, IExaminationTypes
    {
        public ExaminationTypesService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ServiceResponse<ExaminationTypesCreateDtoResponse>> CreateExaminationTypesAsync(ExaminationTypesCreateDtoRequest dto)
        {
            if (await Context.ExaminationTypes.Where(x => x.Name == dto.Name).AnyAsync())
                return new ServiceResponse<ExaminationTypesCreateDtoResponse>(HttpStatusCode.BadRequest, "Podane badanie już istanieje");

            var currentUserName = CurrentlyLoggedUserName;
            if (currentUserName != null)
            {
                var currentUser = await UserManager.FindByNameAsync(currentUserName);
                if (currentUser == null)
                    return new ServiceResponse<ExaminationTypesCreateDtoResponse>(HttpStatusCode.Unauthorized, "Brak uprawnień");

                if (currentUser != null && currentUser.Role != Role.Administrator)
                    return new ServiceResponse<ExaminationTypesCreateDtoResponse>(HttpStatusCode.Forbidden, "Brak uprawnień");
            }

            if (currentUserName == null)
                return new ServiceResponse<ExaminationTypesCreateDtoResponse>(HttpStatusCode.Unauthorized, "Brak uprawnień");

            var examinationType = new ExaminationType()
            {
                Name = dto.Name,
                SpeciesId = dto.SpeciesId
            };

            Context.ExaminationTypes.Add(examinationType);
            int result = await Context.SaveChangesAsync();

            if (result > 0)
            {
                var responseDto = new ExaminationTypesCreateDtoResponse()
                {
                    Name = examinationType.Name,
                    SpeciesId = examinationType.SpeciesId,
                    Id = examinationType.Id
                };

                return new ServiceResponse<ExaminationTypesCreateDtoResponse>(HttpStatusCode.OK, responseDto);
            }

            return new ServiceResponse<ExaminationTypesCreateDtoResponse>(HttpStatusCode.BadRequest);
        }

        public async Task<ServiceResponse<ExaminationTypesGetAllDtoResponse>> GetAllExaminationTypesAsync()
        {
            var currentUserName = CurrentlyLoggedUserName;

            if (currentUserName == null)
                return new ServiceResponse<ExaminationTypesGetAllDtoResponse>(HttpStatusCode.Unauthorized, "Brak uprawnień");

            var currentUser = await UserManager.FindByNameAsync(currentUserName);
            if (currentUser == null)
                return new ServiceResponse<ExaminationTypesGetAllDtoResponse>(HttpStatusCode.Unauthorized, "Brak uprawnień");

            var examinationType = await Context.ExaminationTypes.ToListAsync();

            var dto = new ExaminationTypesGetAllDtoResponse();
            dto.ExaminationTypes = Mapper.Map<List<ExaminationTypesDetailGetAllDtoResponse>>(examinationType);

            return new ServiceResponse<ExaminationTypesGetAllDtoResponse>(HttpStatusCode.OK, dto);
        }

        public async Task<ServiceResponse<ExaminationTypeGetDtoResponse>> GetExaminationTypeAsync(int examinationTypeId)
        {
            var currentUserName = CurrentlyLoggedUserName;

            if (currentUserName == null)
                return new ServiceResponse<ExaminationTypeGetDtoResponse>(HttpStatusCode.Unauthorized, "Brak uprawnień");

            var currentUser = await UserManager.FindByNameAsync(currentUserName);
            if (currentUser == null)
                return new ServiceResponse<ExaminationTypeGetDtoResponse>(HttpStatusCode.Unauthorized, "Brak uprawnień");

            var examinationType = await Context.ExaminationTypes.FindAsync(examinationTypeId);

            if (examinationType == null)
                return new ServiceResponse<ExaminationTypeGetDtoResponse>(HttpStatusCode.BadRequest, "Nie istnieje takie badanie w bazie danych");

            var dto = Mapper.Map<ExaminationTypeGetDtoResponse>(examinationType);

            return new ServiceResponse<ExaminationTypeGetDtoResponse>(HttpStatusCode.OK, dto);
        }
    }
}