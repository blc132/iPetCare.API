﻿using System;
using System.Threading.Tasks;
using Application.Dtos.ExaminationParameters;
using Application.Services.Utilities;

namespace Application.Interfaces
{
    public interface IExaminationParameterService
    {
        Task<ServiceResponse<ExaminationParametersCreateExaminationParameterDtoResponse>> CreateExaminationParameterAsync(ExaminationParametersCreateExaminationParameterDtoRequest dto);
        Task<ServiceResponse<ExaminationParametersGetAllExaminationParametersDtoResponse>> GetAllExaminationParametersAsync();
        Task<ServiceResponse<ExaminationParametersGetExaminationParameterDtoResponse>> GetExaminationParameterAsync(int examinationParameterId);
        Task<ServiceResponse<ExaminationParametersUpdateExaminationParameterDtoResponse>> UpdateExaminationParameterAsync(int examinationParameterId, ExaminationParametersUpdateExaminationParameterDtoRequest dto);
        Task<ServiceResponse> DeleteExaminationParameterAsync(int examinationParameterId);
    }
}
