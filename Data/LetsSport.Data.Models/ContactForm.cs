namespace LetsSport.Data.Models
{
    using LetsSport.Data.Common.Models;

    public class ContactForm : BaseModel<int>
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Ip { get; set; }
    }
}
