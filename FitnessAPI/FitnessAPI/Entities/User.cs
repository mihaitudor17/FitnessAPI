﻿using FitnessAPI.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FitnessAPI.Entities
{
    public class User : IdentityUser
    {
        [JsonIgnore]
        public int PersonId { get; set; }
        [Required]
        public RoleType Role { get; set; }
    }
}
