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
        public int SelectedClockedTimeId { get; set; }
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
        public DateTime EntryStartDate { get; set; }
        [BindProperty]
        public DateTime EntryEndDate { get; set; }
        [BindProperty]
        public List<ClockedTimeReadDto> ClockedTimes { get; set; }
        [BindProperty]
        public int SelectedPositionId { get; set; }
        [BindProperty]
        public int AddVolunteerId { get; set; }
        [BindProperty]
        public int EditVolunteerId { get; set; }

        private readonly IMapper _mapper;

        public TimeClockModel(IMapper mapper, FoodBankContext context, string currentPage = "Time Sheets") : base(context, currentPage)
        {
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage)
        {
            StatusMessage = statusMessage;

            await PrepareModel();

            SearchedStartDate = DateTime.Now.Date;
            SearchedEndDate = DateTime.Now.Date.AddDays(1);
        }

        public async Task<IActionResult> OnPostSaveChanges(int id)
        {
            var clockedTime = await _context.ClockedTime.FirstOrDefaultAsync(ct => ct.Id == id);
            _context.Update(clockedTime);

            var volunteerId = Convert.ToInt32(Request.Form["entry-volunteer-" + id]);
            var positionId = Convert.ToInt32(Request.Form["entry-position-" + id]);
            var startTime = Convert.ToDateTime(Request.Form["entry-date-" + id]);
            var endTime = Convert.ToDateTime(Request.Form["entry-date-" + id]);
            var startTimeStringParts = Request.Form["entry-starttime-" + id].ToString().Split(':');
            var endTimeStringParts = Request.Form["entry-endtime-" + id].ToString().Split(':');

            startTime = startTime.Add(new TimeSpan(Convert.ToInt32(startTimeStringParts[0]), Convert.ToInt32(startTimeStringParts[1]), 0));
            endTime = endTime.Add(new TimeSpan(Convert.ToInt32(endTimeStringParts[0]), Convert.ToInt32(endTimeStringParts[1]), 0));

            clockedTime.Volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == volunteerId);
            clockedTime.Position = await _context.Positions.FirstOrDefaultAsync(v => v.Id == positionId);
            clockedTime.StartTime = startTime;
            clockedTime.EndTime = endTime;

            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"Changes to the entry were successfully saved." }); ;
        }

        public async Task<IActionResult> OnPostAddEntry()
        {
            var volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(p => p.Id == AddVolunteerId);
            var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == SelectedPositionId);

            ClockedTime clock = new ClockedTime()
            {
                Position = position,
                Volunteer = volunteer,
                StartTime = EntryStartDate,
                EndTime = EntryEndDate
            };

            _context.ClockedTime.Add(clock);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"A new entry has been successfully added." });
        }

        public async Task<IActionResult> OnPostDeleteTime()
        {
            var clockedTime = await _context.ClockedTime.FirstOrDefaultAsync(c => c.Id == SelectedClockedTimeId);

            _context.ClockedTime.Remove(clockedTime);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = "The selected entry has been successfully deleted." });
        }

        public async Task OnPostSearch()
        {
            await PrepareModel();

            var searcher = new Searcher(_context);
            ClockedTimes = searcher.FilterTimeSheetBySearch(ClockedTimes, SearchedName, SearchedPosition, SearchedStartDate, SearchedEndDate);
        }

        private async Task PrepareModel()
        {
            var volunteerDomainModels = await _context.VolunteerProfiles.ToListAsync();
            var clockedTimeDomainModels = await _context.ClockedTime
                .Include(p => p.Volunteer)
                .Include(p => p.Position)
                .ToListAsync();

            Positions = await _context.Positions.ToListAsync();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");
            Volunteers = _mapper.Map(volunteerDomainModels, Volunteers);
            ClockedTimes = _mapper.Map(clockedTimeDomainModels, ClockedTimes);
        }
    }
}
