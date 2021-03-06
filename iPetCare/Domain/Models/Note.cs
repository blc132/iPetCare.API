﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Note
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Payload { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ImportantDate { get; set; }

        public Guid PetId { get; set; }
        public virtual Pet Pet { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
