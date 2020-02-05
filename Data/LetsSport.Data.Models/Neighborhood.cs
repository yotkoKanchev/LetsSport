﻿namespace LetsSport.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class Neighborhood : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public City City { get; set; }

        public int CityId { get; set; }
    }
}