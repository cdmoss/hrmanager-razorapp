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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    public class TimeClockModel : AdminPageModel
    {
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public int DeleteClockedTimeId { get; set; }
        [BindProperty]
        public int ClockOutId { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public Position SearchedPosition { get; set; }
        [BindProperty]
        public Position DefaultPosition { get; set; }
        [BindProperty]
        public List<Position> Positions { get; set; }
        [BindProperty]
        public DateTime ManualClockOutTime { get; set; }
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
        public List<ClockedTimeReadDto> CurrentClockedTimes { get; set; }
        [BindProperty]
        public List<ClockedTimeReadDto> CompletedClockedTimes { get; set; }
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
            EntryStartDate = DateTime.Now;
            EntryEndDate = DateTime.Now.AddHours(5);
        }

        public async Task<IActionResult> OnPostSaveChanges(int id)
        {
            var clockedTime = await _context.ClockedTime.FirstOrDefaultAsync(ct => ct.Id == id);
            _context.Update(clockedTime);

            var volunteerId = Convert.ToInt32(Request.Form["entry-volunteer-" + id]);
            var positionId = Convert.ToInt32(Request.Form["entry-position-" + id]);
            var startTime = Convert.ToDateTime(Request.Form["entry-starttime-" + id]);
            var endTime = Convert.ToDateTime(Request.Form["entry-endtime-" + id]);

            startTime = startTime.Add(new TimeSpan(startTime.Hour, startTime.Minute, 0));
            endTime = endTime.Add(new TimeSpan(endTime.Hour, endTime.Minute, 0));

            clockedTime.Volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == volunteerId);
            clockedTime.Position = await _context.Positions.FirstOrDefaultAsync(v => v.Id == positionId);
            clockedTime.StartTime = startTime;
            clockedTime.EndTime = endTime;

            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"Changes to the entry were successfully saved." }); ;
        }

        public async Task<IActionResult> OnPostClockOutVolunteer()
        {
            var clockedTime = await _context.ClockedTime.FirstOrDefaultAsync(ct => ct.Id == ClockOutId);
            _context.Update(clockedTime);

            clockedTime.EndTime = ManualClockOutTime;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { statusMessage = $"Volunteer was successfully clocked out." }); ;
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
            var clockedTime = await _context.ClockedTime.FirstOrDefaultAsync(c => c.Id == DeleteClockedTimeId);

            _context.ClockedTime.Remove(clockedTime);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = "The selected entry has been successfully deleted." });
        }

        public async Task OnPostSearch()
        {
            await PrepareModel();

            var searcher = new Searcher(_context);
            CurrentClockedTimes = searcher.FilterTimeSheetBySearch(CurrentClockedTimes, SearchedName, SearchedPosition, SearchedStartDate, SearchedEndDate);
        }

        private async Task PrepareModel()
        {
            var clockedTimeDtos = new List<ClockedTimeReadDto>();
            var volunteerDomainModels = await _context.VolunteerProfiles.ToListAsync();
            var clockedTimeDomainModels = await _context.ClockedTime
                .Include(p => p.Volunteer)
                .Include(p => p.Position)
                .ToListAsync();

            Positions = await _context.Positions.ToListAsync();
            DefaultPosition = Positions.FirstOrDefault(p => p.Name == "All");
            Volunteers = _mapper.Map(volunteerDomainModels, Volunteers);
            clockedTimeDtos = _mapper.Map(clockedTimeDomainModels, clockedTimeDtos);

            CurrentClockedTimes = clockedTimeDtos.Where(c => c.EndTime == null).ToList();
            CompletedClockedTimes = clockedTimeDtos.Where(c => c.EndTime != null).ToList();
        }
    }
}
