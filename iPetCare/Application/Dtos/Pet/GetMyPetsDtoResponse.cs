﻿using System;
using System.Collections.Generic;
using Domain.Models;

namespace Application.Dtos.Pet
{
    public class GetMyPetsDtoResponse
    {
        public List<PetForGetMyPetsDtoResponse> Pets { get; set; }
    }

    public class PetForGetMyPetsDtoResponse
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int RaceId { get; set; }
    }

}
