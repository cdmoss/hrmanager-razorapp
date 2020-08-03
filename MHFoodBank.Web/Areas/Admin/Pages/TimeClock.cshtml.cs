using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    public class TimeClockModel : AdminPageModel
    {
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public Position SearchedPosition { get; set; }
        [BindProperty]
        public Position DefaultPosition { get; set; }
        [BindProperty]
        public List<Position> Positions { get; set; }
        [BindProperty]
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        [BindProperty]
        public DateTime SearchedStartDate { get; set; }
        [BindProperty]
        public DateTime SearchedEndDate { get; set; }
        [BindProperty]
        public List<ClockedTimeReadDto> ClockedTimes { get; set; }
        [BindProperty]
        public int SelectedPositionId { get; set; }
        [BindProperty]
        public int SelectedVolunteerId { get; set; }

        private readonly IMapper _mapper;

        public TimeClockModel(IMapper mapper, FoodBankContext context, string currentPage = "Time Sheets") : base(context, currentPage)
        {
            _mapper = mapper;
        }

        public async Task OnGet()
        {
            var volunteerDomainModel = await _context.VolunteerProfiles.ToListAsync();

            Positions = await _context.Positions.ToListAsync();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");
            ClockedTimes = new List<ClockedTimeReadDto>();
            Volunteers = _mapper.Map(volunteerDomainModel, Volunteers);
        }
    }
}
