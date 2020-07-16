﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Data.Models;
using MHFoodBank.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    //https://openidauthority.com/how-to-prevent-the-back-button-after-logout-in-asp-net-core/
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class MainModel : AdminPageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        [BindProperty(SupportsGet = true)] 
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        [BindProperty(SupportsGet = true)]
        public Position DefaultPosition { get; set; }
        // make supportsget = true for this will result in it not being null
        [BindProperty] 
        public Position SearchedPosition { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }

        public MainModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMapper mapper, FoodBankContext context, string currentPage = "Volunteers") : base(context, currentPage)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task OnGet()
        {
            var volunteerDomainModels = await PrepareModel();
            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }

        public async Task OnPostSearch()
        {
            var volunteerDomainModels = await PrepareModel();
            Searcher searcher = new Searcher(_context);
            Position searchedPosition = searcher.GetSearchedPosition(Request);
            volunteerDomainModels = searcher.FilterVolunteersBySearch(volunteerDomainModels, SearchedName, searchedPosition);
            Volunteers = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }
        public async Task OnPostDeleteVolunteer()
        {
            int id = Volunteer.Id;
            Volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(p => p.Id == id);
            await _context.Entry(Volunteer).Collection(p => p.Shifts).LoadAsync();

            foreach(Shift shift in Volunteer.Shifts)
            {
                shift.Volunteer = null;
                shift.CreateDescription();
            }

            _context.Remove(Volunteer);
            await _context.SaveChangesAsync();
        }

        private async Task<List<VolunteerProfile>> PrepareModel()
        {
            // get only volunteers
            var volunteersDomainProfiles = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null).ToListAsync();
            Positions = await _context.Positions.ToListAsync();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");

            return volunteersDomainProfiles;
        }
    }
}