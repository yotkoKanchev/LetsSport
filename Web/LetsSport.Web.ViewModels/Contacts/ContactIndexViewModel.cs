﻿namespace LetsSport.Web.ViewModels.Contacts
{
    using System.ComponentModel.DataAnnotations;

    using LetsSport.Data.Models;
    using LetsSport.Services.Mapping;
    using LetsSport.Web.Infrastructure;

    public class ContactIndexViewModel : IMapFrom<ContactForm>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your Full Name")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter titile")]
        [StringLength(100, ErrorMessage = "Title should be between {2} end {1} charachters.", MinimumLength = 5)]
        [Display(Name = "Message title")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Message Content")]
        [StringLength(10000, ErrorMessage = "Message should be at least {2} characters.", MinimumLength = 20)]
        [Display(Name = "Message Content")]
        public string Content { get; set; }

        [GoogleReCaptchaValidation]
        public string RecaptchaValue { get; set; }
    }
}
