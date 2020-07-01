using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using WorkplaceAdministrator.Web.Data;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WorkplaceAdministrator.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [BindProperties]
    public class RegisterModel : PageModel
    {
        private readonly FoodBankContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            FoodBankContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        public VolunteerProfile Volunteer { get; set; }
        public List<Reference> References { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public List<Position> Positions { get; set; }
        public MainCredentials Credentials { get; set; }
        public string AvailabilityError { get; set; }

        public string ReturnUrl { get; set; }

        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            // if we want to add external logins later on
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Positions = _context.Positions.Where(p => p.Name != "All").ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection formData, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            Positions = _context.Positions.ToList();
            // if we want to add external logins later on
            // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                AppUser user = new AppUser 
                { 
                    UserName = Credentials.Email,
                    Email = Credentials.Email,
                    VolunteerProfile = Volunteer
                };

                user.VolunteerProfile.WorkExperiences = WorkExperiences;
                user.VolunteerProfile.References = References;
                user.VolunteerProfile.Availabilities = GetAvailabilitiesFromFormData(formData);
                user.VolunteerProfile.Alerts = new List<Alert>() {new ApplicationAlert{Date = DateTime.Now, Volunteer = user.VolunteerProfile, HasBeenRead = false}};
                user.VolunteerProfile.Positions = AssignPreferredPositions(user.VolunteerProfile);

                IdentityResult accountCreationResult = await TryCreateAccount(user);
                IdentityResult addToRoleResult = await _userManager.AddToRoleAsync(user, "Volunteer");

                // this are initialized to "success" but will be actually evaluated in the methods below
                // string ConfirmationEmailSentResult = "success";

                if (accountCreationResult.Succeeded && 
                    addToRoleResult.Succeeded &&
                    user.VolunteerProfile.Availabilities != null)
                {
                    //TODO: Find out why alerts are being loaded
                    await _context.Entry(user.VolunteerProfile).Collection(p => p.Alerts).LoadAsync();

                    _logger.LogInformation("User created a new account with password.");
                    // we should also figure out if we even need account confirmation
                    //  ConfirmationEmailSentResult = await TrySendConfirmationEmail(user);
                    //  if (ConfirmationEmailSentResult == "success")
                    //  {
                    // this method returns an actionresult
                    return await TrySignInNewAccountAndRedirect(user, returnUrl);
                    // }
                }

                if (!accountCreationResult.Succeeded)
                {
                    AddIdentityErrors(accountCreationResult);   
                }
                if (!addToRoleResult.Succeeded)
                {
                    AddIdentityErrors(addToRoleResult);
                }
                if (user.VolunteerProfile.Availabilities == null)
                {
                    ModelState.AddModelError("AvailabilityError", "One or more of the availability ranges are invalid.");
                }

                // if (ConfirmationEmailSentResult != "success")
                // {
                //     AddAncillaryError(ConfirmationEmailSentResult);
                // }
            }
            var errors = ModelState.Values.Select(v => v.Errors);
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<IdentityResult> TryCreateAccount(AppUser newUser)
        {
            return await _userManager.CreateAsync(newUser, Credentials.Password);
        }

       // private async Task<string> TrySendConfirmationEmail(AppUser user)
       // {
       //     try
       //     {
       //         string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
       //         code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
       //         string callbackUrl = Url.Page(
       //             "/Account/ConfirmEmail",
       //             pageHandler: null,
       //             values: new { area = "Identity", userId = user.Id, code = code },
       //             protocol: Request.Scheme);
       //
       //         await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
       //             $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
       //         return "success";
       //     }
       //     catch (Exception ex)
       //     {
       //         _logger.LogError(ex.ToString());
       //         return "Failed to send confirmation email.";
       //     }
       // }


        // parses the availability form data into Availability objects
        private List<Availability> GetAvailabilitiesFromFormData(IFormCollection formData)
        {
            try
            {
                List<Availability> availabilities = new List<Availability>();
                string[] daysInWeek = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
                for (int i = 0; i < daysInWeek.Length; i++)
                {
                    string currentDay = daysInWeek[i];
                    int fieldCountForCurrentDay = formData.Keys.Count(k => k.Contains($"{currentDay}-1"));

                    for (int j = 1; j <= fieldCountForCurrentDay; j++)
                    {
                        string startTimeString = formData[$"{currentDay}-1-{j}"];
                        string endTimeString = formData[$"{currentDay}-2-{j}"];

                        if (string.IsNullOrWhiteSpace(startTimeString) || string.IsNullOrWhiteSpace(endTimeString))
                        {
                            continue;
                        }

                        string[] startTimeParts = startTimeString.Split(':');
                        string[] endTimeParts = endTimeString.Split(':');

                        TimeSpan startTime = new TimeSpan(int.Parse(startTimeParts[0]), int.Parse(startTimeParts[1]), 0);
                        TimeSpan endTime = new TimeSpan(int.Parse(endTimeParts[0]), int.Parse(endTimeParts[1]), 0);

                        Availability a = new Availability()
                        {
                            AvailableDay = daysInWeek[i],
                            StartTime = startTime,
                            EndTime = endTime,
                        };

                        availabilities.Add(a);
                    }
                }

                return availabilities;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private List<PositionVolunteer> AssignPreferredPositions(VolunteerProfile volunteer)
        {
            List<PositionVolunteer> volunteerPositions = new List<PositionVolunteer>();
            foreach (Position position in Positions)
            {
                var positionWasSelected = Request.Form[position.Name];
              
                // this means it was selected, StringValues are weird
                if (positionWasSelected.Count > 0)
                {
                    PositionVolunteer posVol = new PositionVolunteer()
                    {
                        Volunteer = volunteer,
                        Position = position, 
                        Association = PositionVolunteer.AssociationType.Preferred
                    };
                    volunteerPositions.Add(posVol);
                }
            }

            return volunteerPositions;
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private void AddAncillaryError(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }

        private async Task<IActionResult> TrySignInNewAccountAndRedirect(AppUser user, string returnUrl)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToPage("/Account/Login", new { newUser = true });
        }
    }

    public class MainCredentials
    {
        [Required] 
        [EmailAddress]
        [Compare("ConfirmEmail", ErrorMessage = "The email and confirmation email do not match.")]
        public string Email { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        public string ConfirmEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Your password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Your password must contain at least one letter, one number and one special character (@$!%*#?&)")]
        [Compare("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
