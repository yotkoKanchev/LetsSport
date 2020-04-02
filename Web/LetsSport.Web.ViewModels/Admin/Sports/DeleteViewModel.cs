namespace LetsSport.Web.ViewModels.Admin.Sports
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class DeleteViewModel : IMapFrom<Sport>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
