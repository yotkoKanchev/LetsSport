namespace LetsSport.Web.ViewModels.Admin.Countries
{
    using System;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;

    public class DeleteViewModel : IMapFrom<Country>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
