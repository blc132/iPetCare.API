﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Application.Dtos.Pet;
using Application.Interfaces;
using Application.Services.Utilities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class PetService : Service, IPetService
    {
        public PetService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ServiceResponse<GetPetsDtoResponse>> GetPetsAsync()
        {
            var pets = await Context.Pets.ToListAsync();

            var model = new GetPetsDtoResponse
            {
                Pets = Mapper.Map<List<PetForGetPetsDtoResponse>>(pets)
            };
            return new ServiceResponse<GetPetsDtoResponse>(HttpStatusCode.OK, model);
        }

        public async Task<ServiceResponse<GetPetDtoResponse>> GetPetAsync(Guid petId)
        {
            if(petId == Guid.Empty)
                return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.BadRequest, "Nieprawidłowy Pet Id");

            if(CurrentlyLoggedUser == null)
                return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.Unauthorized);

            var pet = await Context.Pets.SingleOrDefaultAsync(p => p.Id == petId);
            if (pet == null)
                return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.NotFound);

            if (CurrentlyLoggedUser.Role == Role.Owner)
            {
                var owner = await Context.Owners.SingleOrDefaultAsync(o => o.UserId == CurrentlyLoggedUser.Id);
                if (owner == null)
                    return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.Unauthorized);

                if (!await Context.OwnerPets.AnyAsync(op => op.PetId == pet.Id && op.OwnerId == owner.Id))
                    return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.Forbidden);

                var petToReturn = Mapper.Map<GetPetDtoResponse>(pet);
                return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.OK, petToReturn);
            }

            if (CurrentlyLoggedUser.Role == Role.Vet)
            {
                var vet = await Context.Vets.SingleOrDefaultAsync(v => v.UserId == CurrentlyLoggedUser.Id);
                if (vet == null)
                    return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.Unauthorized);

                if (!await Context.VetPets.AnyAsync(vp => vp.PetId == pet.Id && vp.VetId == vet.Id))
                    return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.Forbidden);

                var petToReturn = Mapper.Map<GetPetDtoResponse>(pet);
                return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.OK, petToReturn);
            }

            return new ServiceResponse<GetPetDtoResponse>(HttpStatusCode.Forbidden);
        }

        public async Task<ServiceResponse<CreatePetDtoResponse>> CreatePetAsync(CreatePetDtoRequest dto)
        {
            if(dto.Gender == null)
                return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.BadRequest, "Należy podać płeć");

            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.Unauthorized);

            var owner = await Context.Owners.SingleOrDefaultAsync(o => o.UserId == CurrentlyLoggedUser.Id);
            if (owner == null)
                return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.Unauthorized);

            // if not given from the front
            if (dto.Id == Guid.Empty)
                dto.Id = Guid.NewGuid();

            if(await Context.Pets.AnyAsync(p => p.Id == dto.Id))
                return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.BadRequest, "Istnieje już zwierzak o podanym id.");

            Pet pet = Mapper.Map<Pet>(dto);

            var race = await Context.Races.SingleOrDefaultAsync(r => r.Id == dto.RaceId);
            if (race == null)
                return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.BadRequest, "Nieprawidłowa rasa");

            pet.Race = race;

            Context.Pets.Add(pet);
            Context.OwnerPets.Add(new OwnerPet
            {
                Pet = pet,
                Owner = owner
            });

            if (await Context.SaveChangesAsync() <= 0)
                return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.BadRequest,
                    "Wystąpił błąd podczas tworzenia zwierzaka");

            var petToReturn = Mapper.Map<CreatePetDtoResponse>(pet);
            return new ServiceResponse<CreatePetDtoResponse>(HttpStatusCode.OK, petToReturn);
        }

        public async Task<ServiceResponse<UpdatePetDtoResponse>> UpdatePetAsync(Guid petId,
            UpdatePetDtoRequest dto)
        {
            if (petId == Guid.Empty)
                return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.BadRequest,
                    "Id zwierzaka nie może być pusty");

            if (dto.Gender == null)
                return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.BadRequest,
                    "Należy podać płeć");

            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.Unauthorized);

            var pet = await Context.Pets.SingleOrDefaultAsync(p => p.Id == petId);
            if (pet == null)
                return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.NotFound);

            if (CurrentlyLoggedUser.Role == Role.Owner)
            {
                var owner = await Context.Owners.SingleOrDefaultAsync(o => o.UserId == CurrentlyLoggedUser.Id);
                if (owner == null)
                    return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.Unauthorized);

                if (!await Context.OwnerPets.AnyAsync(op => op.PetId == petId && op.OwnerId == owner.Id))
                    return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.Forbidden);

                Mapper.Map(dto, pet);
                return await Context.SaveChangesAsync() > 0
                    ? new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.OK,
                        Mapper.Map<UpdatePetDtoResponse>(dto))
                    : new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.BadRequest,
                        "Wystąpił błąd podczas aktualizacji zwierzaka");
            }

            if (CurrentlyLoggedUser.Role == Role.Vet)
            {
                var vet = await Context.Vets.SingleOrDefaultAsync(v => v.UserId == CurrentlyLoggedUser.Id);
                if (vet == null)
                    return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.Unauthorized);

                if (!await Context.VetPets.AnyAsync(vp => vp.PetId == petId && vp.VetId == vet.Id))
                    return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.Forbidden);

                Mapper.Map(dto, pet);
                return await Context.SaveChangesAsync() > 0
                    ? new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.OK,
                        Mapper.Map<UpdatePetDtoResponse>(dto))
                    : new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.BadRequest,
                        "Wystąpił błąd podczas aktualizacji zwierzaka");

            }

            if (CurrentlyLoggedUser.Role == Role.Administrator)
            {
                Mapper.Map(dto, pet);
                return await Context.SaveChangesAsync() > 0
                    ? new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.OK,
                        Mapper.Map<UpdatePetDtoResponse>(dto))
                    : new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.BadRequest,
                        "Wystąpił błąd podczas aktualizacji zwierzaka");
            }
            return new ServiceResponse<UpdatePetDtoResponse>(HttpStatusCode.Forbidden);
        }

        public async Task<ServiceResponse> DeletePetAsync(Guid petId)
        {
            if (petId == Guid.Empty)
                return new ServiceResponse(HttpStatusCode.BadRequest, "Nieprawidłowy id zwierzaka");

            if (CurrentlyLoggedUser == null)
                return new ServiceResponse(HttpStatusCode.Unauthorized);

            var owner = await Context.Owners.SingleOrDefaultAsync(o => o.UserId == CurrentlyLoggedUser.Id);
            if (owner == null)
                return new ServiceResponse(HttpStatusCode.Unauthorized);

            var pet = await Context.Pets.SingleOrDefaultAsync(p => p.Id== petId);
            if (pet == null)
                return new ServiceResponse(HttpStatusCode.NotFound);

            if (!await Context.OwnerPets.AnyAsync(op => op.OwnerId == owner.Id && op.PetId == petId))
                return new ServiceResponse(HttpStatusCode.Forbidden);

            Context.Pets.Remove(pet);
            return await Context.SaveChangesAsync() > 0 ? new ServiceResponse(HttpStatusCode.OK) : new ServiceResponse(HttpStatusCode.BadRequest, "Wystąpił błąd podczas usuwania zwierzaka");
        }

        public async Task<ServiceResponse<GetMyPetsDtoResponse>> GetMyPetsAsync()
        {
            if(CurrentlyLoggedUser == null)
                return new ServiceResponse<GetMyPetsDtoResponse>(HttpStatusCode.Unauthorized);

            var pets = await Context.Pets
                .Where(x => x.OwnerPets
                    .Any(y => y.OwnerId == CurrentlyLoggedUser.Owner.Id))
                .ToListAsync();

            var dto = new GetMyPetsDtoResponse();
            dto.Pets = Mapper.Map<List<PetForGetMyPetsDtoResponse>>(pets);

            return new ServiceResponse<GetMyPetsDtoResponse>(HttpStatusCode.OK, dto);
        }

        public async Task<ServiceResponse<GetSharedPetsDtoResponse>> GetSharedPetsAsync()
        {
            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<GetSharedPetsDtoResponse>(HttpStatusCode.Unauthorized);

            if (CurrentlyLoggedUser == null)
                return new ServiceResponse<GetSharedPetsDtoResponse>(HttpStatusCode.Unauthorized);

            var pets = await Context.Requests
                .Where(x => x.IsAccepted && x.User.Id == CurrentlyLoggedUser.Id)
                .Select(x => x.Pet)
                .ToListAsync();

            var dto = new GetSharedPetsDtoResponse();
            dto.Pets = Mapper.Map<List<PetForGetSharedPetsDtoResponse>>(pets);

            return new ServiceResponse<GetSharedPetsDtoResponse>(HttpStatusCode.OK, dto);
        }
    }
}