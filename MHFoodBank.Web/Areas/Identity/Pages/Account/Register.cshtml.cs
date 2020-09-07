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
        private readonly IEmailConfirm _emailConfirm;
        private readonly IMapper _mapper;

        public RegisterModel(
            FoodBankContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            ILogger<RegisterModel> logger,
            IEmailConfirm emailConfirm)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _emailConfirm = emailConfirm;
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

            IdentityResult accountCreationResult = null;
            IdentityResult addToRoleResult = null;

            // this are initialized to "success" but will be actually evaluated in the methods below
            bool ConfirmationEmailSentResult = true;

            if (user.VolunteerProfile.Availabilities != null)
            {
                accountCreationResult = await _userManager.CreateAsync(user, Volunteer.Password);
                addToRoleResult = await _userManager.AddToRoleAsync(user, "Volunteer");

                if (accountCreationResult.Succeeded && addToRoleResult.Succeeded)
                {
                    await _context.Entry(user.VolunteerProfile).Collection(p => p.Alerts).LoadAsync();

                    _logger.LogInformation("User created a new account with password.");
                    // we should also figure out if we even need account confirmation

                    try
                    {
                        await TrySendConfirmationEmail(user);
                        return RedirectToPage("/Account/Login", new { registeredUserId = user.Id, statusMessage = "Thank you for applying to the Medicine Hat Food Bank! You will receive a confirmation email to confirm your account. Check your spam/junk folder if the email is not in your inbox." });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(String.Empty, "Email failed to send.");

                    }
                }
                else
                {
                    if (accountCreationResult != null && !accountCreationResult.Succeeded)
                    {
                        if (_context.Users.Any(e => e.Email == Volunteer.Email))
                        {
                            ModelState.AddModelError("EmailError", "This email is already in use.");
                        }

                        AddIdentityErrors(accountCreationResult);
                    }
                    if (addToRoleResult != null && !addToRoleResult.Succeeded)
                    {
                        AddIdentityErrors(addToRoleResult);
                    }
                }
            }
            else
            {
                if (user.VolunteerProfile.Availabilities == null)
                {
                    ModelState.AddModelError("AvailabilityError", "One or more of the availability ranges are invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task TrySendConfirmationEmail(AppUser user)
        {
            try
            {
                await _emailConfirm.TrySendConfirmationEmail(user, Request, Url);
            }
            catch (Exception ex)
            {
                _logger.LogError("Email confirmation failed to send. Error: " + ex.ToString());
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
