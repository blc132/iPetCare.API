﻿using System;

namespace Application.Dtos.Examinations
{
    public class UpdateExaminationDtoResponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int ExaminationTypeId { get; set; }
        public string Content { get; set; }
        public Guid PetId { get; set; }
    }
}
