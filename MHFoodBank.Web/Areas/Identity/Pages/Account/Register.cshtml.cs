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
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Services;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MHFoodBank.Common.Dtos;
using AutoMapper;

namespace MHFoodBank.Web.Areas.Identity.Pages.Account
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
        private readonly IMapper _mapper;

        public RegisterModel(
            FoodBankContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        [BindProperty]
        public RegisterDto Volunteer { get; set; } = new RegisterDto();
        public List<ReferenceDto> References { get; set; } = new List<ReferenceDto>() { new ReferenceDto(), new ReferenceDto() };
        public List<WorkExperienceDto> WorkExperiences { get; set; } = new List<WorkExperienceDto>() { new WorkExperienceDto(), new WorkExperienceDto() };
        public List<Position> Positions { get; set; }
        public string AvailabilityError { get; set; }
        public string EmailError { get; set; }

        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public IActionResult OnGet()
        {
            // if we want to add external logins later on
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Positions = _context.Positions.Where(p => p.Name != "All" && !p.Deleted).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormCollection formData)
        {
            Positions = _context.Positions.ToList();
            // if we want to add external logins later on
            // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
            {
                return OnGet();
            }

            Volunteer.WorkExperiences = WorkExperiences;
            Volunteer.References = References;
            Volunteer.ApprovalStatus = ApprovalStatus.Pending;

            var domainVolunteerProfile = _mapper.Map<VolunteerProfile>(Volunteer);

            AppUser user = new AppUser
            {
                UserName = Volunteer.Email,
                Email = Volunteer.Email,
                VolunteerProfile = domainVolunteerProfile
            };
            var availHandler = new AvailabilityHandler();
            user.VolunteerProfile.Availabilities = availHandler.GetAvailabilitiesFromFormData(formData, user.VolunteerProfile);
            user.VolunteerProfile.Alerts = new List<Alert>() { new ApplicationAlert { Date = DateTime.Now, Volunteer = user.VolunteerProfile, Read = false } };
            user.VolunteerProfile.Positions = AssignPreferredPositions(user.VolunteerProfile);

            IdentityResult accountCreationResult = await _userManager.CreateAsync(user, Volunteer.Password);
            IdentityResult addToRoleResult = await _userManager.AddToRoleAsync(user, "Volunteer");

            // this are initialized to "success" but will be actually evaluated in the methods below
            bool ConfirmationEmailSentResult = true;

            if (accountCreationResult.Succeeded &&
                addToRoleResult.Succeeded &&
                user.VolunteerProfile.Availabilities != null)
            {
                await _context.Entry(user.VolunteerProfile).Collection(p => p.Alerts).LoadAsync();

                _logger.LogInformation("User created a new account with password.");
                // we should also figure out if we even need account confirmation
                ConfirmationEmailSentResult = await TrySendConfirmationEmail(user);
                if (ConfirmationEmailSentResult)
                {
                    return RedirectToPage("/Account/Login", new { statusMessage = "Thank you for applying to the Medicine Hat Food Bank! You will receive a confirmation email to confirm your account. Check your spam/junk folder if the email is not in your inbox." });
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Email failed to send");
                }
            }
            else
            {
                if (!accountCreationResult.Succeeded)
                {
                    if (_context.Users.Any(e => e.Email == Volunteer.Email))
                    {
                        ModelState.AddModelError("EmailError", "This email is already in use.");
                    }

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
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<bool> TrySendConfirmationEmail(AppUser user)
        {
            try
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                string callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "MHFB - Confirm your email",
                    $"<img alt='Medicine Hat Food Bank Logo' src='https://static.wixstatic.com/media/9c0c8c_8e19160d960f483f9252fcfd8b45af4a~mv2_d_8359_7389_s_4_2.png/v1/fill/w_120,h_106,al_c,q_85,usm_0.66_1.00_0.01/9c0c8c_8e19160d960f483f9252fcfd8b45af4a~mv2_d_8359_7389_s_4_2.webp'><br/><br/>Thanks for applying as a volunteer to the Medicine Hat Food Bank!<br/><br/>Once you confirm your account, you will be able to login to manage your availability and personal information. Once you're accepted you will be able to view and sign up for open shifts.<br/><br/>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Email confirmation failed to send. Error: " + ex.ToString());
                return false;
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
    }
}
