namespace LetsSport.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Common.Models;

    public class ContactForm : BaseModel<int>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(10000)]
        public string Content { get; set; }

        public string Ip { get; set; }

        public bool IsReplyed { get; set; }

        [MinLength(5)]
        [MaxLength(10000)]
        public string Reply { get; set; }

        public DateTime? ReplyedOn { get; set; }

        [MinLength(20)]
        [MaxLength(10000)]
        public string ReplyContent { get; set; }
    }
}
