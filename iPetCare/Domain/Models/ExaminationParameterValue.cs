﻿using System;

namespace Domain.Models
{
    public class ExaminationParameterValue
    {
        public Guid Id { get; set; }
        public float Value { get; set; }

        public int ExaminationParameterId { get; set; }
        public virtual ExaminationParameter ExaminationParameter { get; set; }

        public Guid ExaminationId { get; set; }
        public virtual Examination Examination{ get; set; }
    }
}
