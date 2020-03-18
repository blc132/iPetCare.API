﻿using System.Threading.Tasks;
using Application.Dtos.Races;
using Application.Services.Utilities;

namespace Application.Interfaces
{
    public interface IRaceService
    {
        Task<ServiceResponse<CreateDtoResponse>> CreateAsync(CreateDtoRequest dto);
    }
}
