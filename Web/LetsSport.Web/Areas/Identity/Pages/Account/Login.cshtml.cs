﻿// <auto-generated />
namespace LetsSport.Web.Areas.Identity.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using LetsSport.Common;
    using LetsSport.Data.Models;
    using LetsSport.Web.Infrastructure;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILocationLocator locator;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LoginModel> logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager, ILocationLocator locator)
        {
            this.userManager = userManager;
            this.locator = locator;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            if (this.User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Home/Error");
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.Input.EmailOrUsername.IndexOf('@') > -1)
            {
                //Validate email format
                string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                       @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                          @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(this.Input.EmailOrUsername))
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            else
            {
                //validate Username format
                string emailRegex = @"^[a-zA-Z0-9]*$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(this.Input.EmailOrUsername))
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var userName = this.Input.EmailOrUsername;

                // This if the user would like to loging with username or email
                if (userName.IndexOf('@') > -1)
                {
                    var user = await this.userManager.FindByEmailAsync(this.Input.EmailOrUsername);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        //return this.View(this.Input);
                    }
                    else
                    {
                        userName = user.UserName;
                    }
                }

                var result = await this.signInManager.PasswordSignInAsync(userName, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");
                    this.SetLocation();
                    return this.LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = this.Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");

                    return this.RedirectToPage("./Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                    return this.Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        protected void SetLocation()
        {
            string currentCity;
            string currentCountry;

            if (this.HttpContext.Session.GetString("city") != null)
            {
                this.ViewData["location"] = this.HttpContext.Session.GetString("location");
                currentCity = this.HttpContext.Session.GetString("city");
                currentCountry = this.HttpContext.Session.GetString("country");
            }
            else
            {
                var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
                var currentLocation = this.locator.GetLocationInfo(ip);
                currentCity = currentLocation.City;
                currentCountry = currentLocation.Country;
                var location = currentLocation.City + ", " + currentLocation.Country;

                this.HttpContext.Session.SetString("city", currentCity);
                this.HttpContext.Session.SetString("country", currentCountry);
                this.HttpContext.Session.SetString("location", location);

                this.ViewData["location"] = this.HttpContext.Session.GetString("location");
            }
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Email or Username")]
            public string EmailOrUsername { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
    }
}
