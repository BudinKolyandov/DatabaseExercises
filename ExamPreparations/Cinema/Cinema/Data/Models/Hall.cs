﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Data.Models
{
    public class Hall
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        public bool Is4Dx { get; set; }

        public bool Is3D { get; set; }

        public ICollection<Projection> Projections { get; set; } = new HashSet<Projection>();
        
        public ICollection<Seat> Seats { get; set; } = new HashSet<Seat>();
    }
}