﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public string ImageUrl { get; set; }

        public virtual Vet Vet{ get; set; }

        public virtual Owner Owner{ get; set; }

        public virtual ICollection<Request> Requests { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    }
}