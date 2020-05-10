﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Institutions
{
    public class CreateInstitutionDtoRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [MaxLength(255, ErrorMessage = "Długość nie może być większa, niż 255 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        [MaxLength(255, ErrorMessage = "Długość nie może być większa, niż 255 znaków")]
        public string Address { get; set; }
    }
}
