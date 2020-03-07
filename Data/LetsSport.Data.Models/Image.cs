namespace LetsSport.Data.Models
{
    using System;

    using LetsSport.Data.Common.Models;
    using LetsSport.Data.Models.ArenaModels;
    using LetsSport.Data.Models.UserModels;

    public class Image : BaseDeletableModel<string>
    {
        public Image()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Url { get; set; }

        public virtual Arena Arena { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}
