using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Areas.Volunteer.Pages.Shared;
using WorkplaceAdministrator.Web.Data;
using WorkplaceAdministrator.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WorkplaceAdministrator.Web.Areas.Volunteer.Pages
{
    public class ShiftRequestsModel : VolunteerPageModel
    {
        public List<ShiftRequestAlert> Alerts { get; set; }
        public ShiftRequestsModel(FoodBankContext context, UserManager<AppUser> userManager) : base(userManager,
            context)
        {

        }

        public async Task OnGet()
        {
            await PrepareModel();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {

            ShiftRequestAlert selectedRequestAlert = await _context.ShiftAlerts.FirstOrDefaultAsync(a => a.Id == id);

            await _context.Entry(selectedRequestAlert).Reference(p => p.OldShift).LoadAsync();
            await _context.Entry(selectedRequestAlert).Reference(p => p.NewShift).LoadAsync();

            if (selectedRequestAlert.DismissedByAdmin)
            {
                _context.Remove(selectedRequestAlert);
                await _context.SaveChangesAsync();
                _context.Remove(selectedRequestAlert.OldShift);
                _context.Remove(selectedRequestAlert.NewShift);
            }
            else
            {
                _context.Alerts.Update(selectedRequestAlert);
                selectedRequestAlert.DismissedByVolunteer = true;
            }

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
            Alerts = await _context.ShiftAlerts
                .Include(p => p.OldShift)
                .Include(p => p.NewShift)
                .Where(sa => sa.DismissedByVolunteer == false && sa.Volunteer.Id == currentUser.VolunteerProfile.Id).ToListAsync();

            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
        }
    }
}