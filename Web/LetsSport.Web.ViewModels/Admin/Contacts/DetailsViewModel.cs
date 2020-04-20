namespace LetsSport.Web.ViewModels.Admin.Contacts
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class DetailsViewModel : IMapFrom<ContactForm>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Content { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(10000)]
        public string ReplyContent { get; set; }
    }
}
