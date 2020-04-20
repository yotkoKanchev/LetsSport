namespace LetsSport.Web.ViewModels.Admin.Contacts
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class ContactInfoViewModel : IMapFrom<ContactForm>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
