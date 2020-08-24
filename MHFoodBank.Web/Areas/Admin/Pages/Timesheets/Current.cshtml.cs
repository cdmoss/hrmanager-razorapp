using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using MHFoodBank.Web.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Web.Areas.Admin.Pages.TimeClock
{
    public class CurrentModel : AdminPageModel
    {
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public int SelectedClockedTimeId { get; set; }
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
        public List<ClockedTimeReadDto> ClockedTimes { get; set; }
        [BindProperty]
        public string SelectedPositionName { get; set; }
        [BindProperty]
        public string VolunteerNameForAdd { get; set; }
        [BindProperty]
        public string PositionNameForAdd { get; set; }

        private readonly IMapper _mapper;

        public CurrentModel(IMapper mapper, FoodBankContext context, string currentPage = "Time Sheets") : base(context, currentPage)
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

            string volunteerIdStr = Request.Form["entry-volunteer-" + id][0].Substring(0, 1);
            int volunteerId = 0;
            if (int.TryParse(volunteerIdStr, out int volIdParsed))
            {
                volunteerId = volIdParsed;
            }
            else
            {
                return RedirectToPage(new { statusMessage = $"Error: Volunteer not found." });
            }
            string positionName = Request.Form["entry-position-" + id][0];
            Positions = await _context.Positions.ToListAsync();
            if (!Positions.Any(p => p.Name == positionName))
            {
                return RedirectToPage(new { statusMessage = $"Error: Position not found." });
            }
            var startTime = Convert.ToDateTime(Request.Form["entry-starttime-" + id]);

            clockedTime.Volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.Id == volunteerId);
            clockedTime.Position = await _context.Positions.FirstOrDefaultAsync(v => v.Name == positionName);
            clockedTime.StartTime = startTime;

            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"Changes to the entry were successfully saved." });
        }

        public async Task<IActionResult> OnPostClockOutVolunteer()
        {
            var clockedTime = await _context.ClockedTime.FirstOrDefaultAsync(ct => ct.Id == SelectedClockedTimeId);
            _context.Update(clockedTime);

            clockedTime.EndTime = ManualClockOutTime;

            await _context.SaveChangesAsync();
            return RedirectToPage(new { statusMessage = $"Volunteer was successfully clocked out." }); ;
        }

        public async Task<IActionResult> OnPostAddEntry()
        {
            var volunteerName = VolunteerNameForAdd;
            var volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(p => p.FullNameWithID == VolunteerNameForAdd);
            var position = await _context.Positions.FirstOrDefaultAsync(p => p.Name == PositionNameForAdd);

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
            ClockedTimes = searcher.FilterTimeSheetBySearch(ClockedTimes, SearchedName, SearchedPosition);
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

            ClockedTimes = clockedTimeDtos.Where(c => c.EndTime == null).ToList();
        }
    }
}
