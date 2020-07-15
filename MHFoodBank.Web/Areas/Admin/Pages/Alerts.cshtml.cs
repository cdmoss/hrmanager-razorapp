using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using MHFoodBank.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [BindProperties]
    [Authorize(Roles = "Staff, Admin")]
    public class AlertsModel : AdminPageModel
    {
        private readonly IMapper _mapper;

        public List<AdminAlertListDto> PendingRequests { get; set; }
        public List<AdminAlertListDto> ArchivedRequests { get; set; }
        public List<AdminAlertListDto> ApplicationAlerts { get; set; }
        public string SearchedName { get; set; }
        public string StatusMessage { get; set; }

        public AlertsModel(FoodBankContext context,IMapper mapper, string currentPage = "Alerts") : base(context, currentPage)
        {
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage = null)
        {
            StatusMessage = statusMessage;
            await PrepareModel();
        }

        public async Task<IActionResult> OnPostViewApplicant(int id)
        {
            var selectedAlert = await _context.Alerts.FirstOrDefaultAsync(a => a.Id == id);
            //TODO: This is throwing a null error
            await _context.Entry(selectedAlert).Reference(a => a.Volunteer).LoadAsync();
            _context.Alerts.Update(selectedAlert);
            selectedAlert.Read = true;
            await _context.SaveChangesAsync();
            return RedirectToPage("VolunteerDetails", new { id = selectedAlert.Volunteer.Id });
        }

        public async Task<IActionResult> OnPostViewRequest(int id)
        {
            var selectedAlert = await _context.Alerts.FirstOrDefaultAsync(a => a.Id == id);
            await _context.Entry(selectedAlert).Reference(a => a.Volunteer).LoadAsync();
            _context.Alerts.Update(selectedAlert);
            selectedAlert.Read = true;
            await _context.SaveChangesAsync();
            return RedirectToPage("ResolveShiftRequest", new { alertId = selectedAlert.Id });
        }

        public async Task<IActionResult> OnPostDeleteAlert(int id)
        {
            var selectedAlert = await _context.Alerts.FirstOrDefaultAsync(a => a.Id == id);
            _context.Update(selectedAlert);
            if (selectedAlert is ShiftRequestAlert shiftAlert)
            {
                await _context.Entry(shiftAlert).Reference(p => p.OriginalShift).LoadAsync();
                await _context.Entry(shiftAlert).Reference(p => p.RequestedShift).LoadAsync();

                if (shiftAlert.DismissedByVolunteer)
                {
                    shiftAlert.Deleted = true;
                }
                else
                {
                    shiftAlert.DismissedByAdmin = true;
                }
            }
            else
            {
                selectedAlert.Deleted = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        private async Task PrepareModel()
        {
            var shiftRequests = await _context.ShiftAlerts
                .Include(p => p.Volunteer)
                .Where(a => !a.DismissedByAdmin && !a.Deleted )
                .ToListAsync();

            PendingRequests = _mapper.Map<List<AdminAlertListDto>>(shiftRequests
                    .Where(sr => sr.Status == ShiftRequestAlert.RequestStatus.Pending)).ToList();

            ArchivedRequests = _mapper.Map<List<AdminAlertListDto>>(shiftRequests
                    .Where(sr => sr.Status != ShiftRequestAlert.RequestStatus.Pending)).ToList();


            var applicationAlerts = await _context.ApplicationAlerts
                .Include(p => p.Volunteer)
                .Where(a => !a.Deleted)
                .ToListAsync();

            ApplicationAlerts = _mapper.Map<List<AdminAlertListDto>>(applicationAlerts).ToList();

            PendingRequests = PendingRequests.OrderByDescending(a => a.Date).ToList();
            ArchivedRequests = ArchivedRequests.OrderByDescending(a => a.Date).ToList();
            ApplicationAlerts = ApplicationAlerts.OrderByDescending(a => a.Date).ToList();
        }

        //public async Task OnPostSearch()
        //{
        //    if (SearchedName != null)
        //    {
        //        Alerts = await _context.Alerts
        //            .Include(p => p.User)
        //            .ThenInclude(u => u.VolunteerProfile)
        //            .ToListAsync();
        //        Alerts = Alerts.Where(a => a.User.VolunteerProfile.FullNameWithID.Contains(SearchedName)).ToList();
        //    }
        //    else
        //    {
        //        Alerts = await _context.Alerts
        //            .Include(p => p.User)
        //            .ThenInclude(u => u.VolunteerProfile)
        //            .ToListAsync();
        //    }
        //
        //    Alerts = Alerts.OrderByDescending(a => a.Date).ToList();
        //}
    }
}