using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public double Weight { get; set; }

        public double Price { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<FoodOrder> Orders { get; set; } = new HashSet<FoodOrder>();
    }
}
