﻿using System;
using System.Threading.Tasks;
using Application.Dtos.ExaminationTypes;
using Application.Services.Utilities;

namespace Application.Interfaces
{
    public interface IExaminationTypeService
    {
        Task<ServiceResponse<CreateExaminationTypeDtoResponse>> CreateExaminationTypeAsync(CreateExaminationTypeDtoRequest dto);
        Task<ServiceResponse<GetAllExaminationTypesDtoResponse>> GetAllExaminationTypesAsync();
        Task<ServiceResponse<UpdateExaminationTypeDtoResponse>> UpdateExaminationTypeAsync(int examinationTypeId, UpdateExaminationTypeDtoRequest dto);
        Task<ServiceResponse> DeleteExaminationTypeAsync(int examinationTypeId);
        Task<ServiceResponse<ExaminationParametersGetAllForOneExaminationTypeDtoResponse>> GetAllForOneExaminationTypeAsync(int examinationTypeId);
        Task<ServiceResponse<GetExaminationTypesByPetIdResponse>> GetExaminationTypesByPetIdAsync(Guid petId);
    }
}
