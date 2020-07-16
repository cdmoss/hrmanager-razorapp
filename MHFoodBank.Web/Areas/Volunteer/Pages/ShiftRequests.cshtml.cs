﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Web.Areas.Volunteer.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using MHFoodBank.Web.Dtos;
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
        public ShiftRequestsModel(FoodBankContext context, IMapper mapper, UserManager<AppUser> userManager) : base(userManager,
            context)
        {
            _mapper = mapper;
        }

        public async Task OnGet()
        {
            await PrepareModel();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {

            ShiftRequestAlert selectedRequestAlert = await _context.ShiftAlerts.FirstOrDefaultAsync(a => a.Id == id);

            await _context.Entry(selectedRequestAlert).Reference(p => p.OriginalShift).LoadAsync();
            await _context.Entry(selectedRequestAlert).Reference(p => p.RequestedShift).LoadAsync();

            if (selectedRequestAlert.DismissedByAdmin)
            {
                _context.Remove(selectedRequestAlert);
                await _context.SaveChangesAsync();
                _context.Remove(selectedRequestAlert.OriginalShift);
                _context.Remove(selectedRequestAlert.RequestedShift);
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
            var alertDomainModels = await _context.ShiftAlerts
                .Include(p => p.OriginalShift)
                .Include(p => p.RequestedShift)
                .Where(sa => sa.DismissedByVolunteer == false && sa.Volunteer.Id == currentUser.VolunteerProfile.Id).ToListAsync();

            Alerts = _mapper.Map<List<ShiftRequestReadDto>>(alertDomainModels);

            LoggedInUser = currentUser.VolunteerProfile.FirstName + " " + currentUser.VolunteerProfile.LastName;
        }
    }
}