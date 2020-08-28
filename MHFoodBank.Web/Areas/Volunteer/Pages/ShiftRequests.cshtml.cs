using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    public class ShiftRequestsModel : VolunteerPageModel
    {
        private readonly IMapper _mapper;
        public List<ShiftRequestReadDto> Alerts { get; set; }
        public ShiftRequestsModel(FoodBankContext context, IMapper mapper, UserManager<AppUser> userManager, string currentPage = "Shift Requests") : base(userManager,
            context, currentPage)
        {
            _mapper = mapper;
        }

        public async Task OnGet()
        {
            await PrepareModel();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {

            ShiftRequestAlert selectedRequestAlert = await _context.ShiftAlerts
                .Include(p => p.OriginalShift)
                .Include(p => p.RequestedShift)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (selectedRequestAlert.DismissedByAdmin)
            {
                _context.Remove(selectedRequestAlert);
            }
            else
            {
                _context.Alerts.Update(selectedRequestAlert);
                selectedRequestAlert.DismissedByVolunteer = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var alert = await _context.ShiftAlerts.FirstOrDefaultAsync(a => a.Id == id);
            _context.Alerts.Remove(alert);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public IActionResult OnPostCalendar()
        {
            return RedirectToPage("VolunteerCalendar");
        }

        private async Task PrepareModel()
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);
            await _context.Entry(currentUser).Reference(p => p.VolunteerProfile).LoadAsync();
            await _context.Entry(currentUser.VolunteerProfile).Collection(p => p.Shifts).LoadAsync();
            var alertDomainModels = await _context.ShiftAlerts
                .Include(p => p.OriginalShift)
                .Include(p => p.RequestedShift)
                .Where(sa => sa.DismissedByVolunteer == false && sa.Volunteer.Id == currentUser.VolunteerProfile.Id).ToListAsync();

            Alerts = _mapper.Map<List<ShiftRequestReadDto>>(alertDomainModels);

            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
        }
    }
}