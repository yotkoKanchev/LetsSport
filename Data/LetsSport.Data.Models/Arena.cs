namespace LetsSport.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using LetsSport.Data.Common.Models;

    public class Arena : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Address { get; set; }

        public string PricePerHour { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string WebUrl { get; set; }

        public double Rating { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
    }
}
