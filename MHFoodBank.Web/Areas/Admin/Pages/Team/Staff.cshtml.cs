using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Syncfusion.EJ2.Base;

namespace MHFoodBank.Web.Areas.Admin.Pages.Teams
{
    [Authorize(Roles = "Staff, Admin")]
    public class StaffModel : AdminPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        [BindProperty]
        public bool ArchivedFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<VolunteerMinimalDto> Staff { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Position> Positions { get; set; }
        // make supportsget = true for this will result in it not being null
        [BindProperty]
        public int SearchedPositionId { get; set; }
        [BindProperty]
        public string SearchedName { get; set; }
        [BindProperty]
        public int SelectedStaffId { get; set; }
        [BindProperty]
        public string StatusMessage { get; set; }
        [BindProperty]
        public StaffRegisterDto NewStaff { get; set; } = new StaffRegisterDto();

        public StaffModel(UserManager<AppUser> userManager, IMapper mapper, FoodBankContext context, string currentPage = "Staff") : base(context, currentPage)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task OnGet(string statusMessage = null, bool archivedFilter = false)
        {
            StatusMessage = statusMessage;
            var volunteerDomainModels = await PrepareModel(statusMessage, archivedFilter);
            Staff = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }

        public async Task OnPost()
        {
            await OnGet("", ArchivedFilter);
        }

        public class Data
        {
            public bool requiresCounts { get; set; }
            public int skip { get; set; }

            public int EmployeeID { get; set; }
            public int EmployeeName { get; set; }
            public int take { get; set; }
            [HtmlAttributeName("sorted")]
            [JsonProperty("sorted")]
            public List<Sort> Sorted { get; set; }
            [HtmlAttributeName("search")]
            [JsonProperty("search")]
            public List<SearchFilter> Search { get; set; }
            [HtmlAttributeName("where")]
            [JsonProperty("where")]
            public List<WhereFilter> Where { get; set; }
        }

        public async Task<JsonResult> OnPostDelete([FromBody] CRUDModel<VolunteerAdminReadEditDto> crudRequest)
        {
            if(crudRequest.Action == "remove")
            {
                var selectedStaffId = JsonConvert.DeserializeObject<VolunteerAdminReadEditDto>(crudRequest.Key.ToString()).Id;
                var selectedStaff = await _context.VolunteerProfiles.FirstOrDefaultAsync(s => s.Id == selectedStaffId);
                await _context.Entry(selectedStaff).Reference(u => u.User).LoadAsync();
                var selectedStaffUserLogin = await _context.Users.FirstOrDefaultAsync(u => u.Id == selectedStaff.User.Id);
                _context.VolunteerProfiles.Remove(selectedStaff);
                _context.Users.Remove(selectedStaffUserLogin);
            }
            else if(crudRequest.Action == "batch")
            {
                foreach (var staff in crudRequest.Deleted)
                {
                    var selectedStaff = await _context.VolunteerProfiles.FirstOrDefaultAsync(s => s.Id == staff.Id);
                    await _context.Entry(selectedStaff).Reference(u => u.User).LoadAsync();
                    var selectedStaffUserLogin = await _context.Users.FirstOrDefaultAsync(u => u.Id == selectedStaff.User.Id);
                    _context.VolunteerProfiles.Remove(selectedStaff);
                    _context.Users.Remove(selectedStaffUserLogin);
                }
            }

            await _context.SaveChangesAsync();

            return await OnPostGetStaff(new Data());
        }

        public async Task<JsonResult> OnPostGetStaff([FromBody] Data dm)
        {
            
            var allStaff = await _context.VolunteerProfiles.Include(p => p.User).Where(s => s.IsStaff).ToListAsync();

            if (dm.Search != null)
            {
                allStaff = allStaff.Where(s => s.FirstName.ToLower().Contains(dm.Search[0].Key.ToLower()) ||
                                               s.LastName.ToLower().Contains(dm.Search[0].Key.ToLower()) ||
                                               s.User.Email.ToLower().Contains(dm.Search[0].Key.ToLower())).ToList();
            }

            if (dm.Where != null)
            {
                foreach (var predicate in dm.Where[0].predicates)
                {
                    if (predicate.Field == "DisplayStatus")
                    {
                        allStaff = allStaff.Where(f => f.DisplayStatus == predicate.value.ToString()).ToList();
                    }
                }
            }

            if (dm.Sorted != null)
            {
                foreach (var sortCondition in dm.Sorted)
                {
                    if (sortCondition.Name == "FirstName")
                    {
                        if (sortCondition.Direction == "ascending")
                        {
                            allStaff = allStaff.OrderBy(s => s.FirstName).ToList();
                        }
                        else
                        {
                            allStaff = allStaff.OrderByDescending(s => s.FirstName).ToList();
                        }
                    }
                    if (sortCondition.Name == "LastName")
                    {
                        if (sortCondition.Direction == "ascending")
                        {
                            allStaff = allStaff.OrderBy(s => s.LastName).ToList();
                        }
                        else
                        {
                            allStaff = allStaff.OrderByDescending(s => s.LastName).ToList();
                        }
                    }
                    else if (sortCondition.Name== "Email")
                    {
                        if (sortCondition.Direction == "ascending")
                        {
                            allStaff = allStaff.OrderBy(s => s.User.Email).ToList();
                        }
                        else
                        {
                            allStaff = allStaff.OrderByDescending(s => s.User.Email).ToList();
                        }
                    }
                }
            }


            var dtos = _mapper.Map<List<VolunteerAdminReadEditDto>>(allStaff);

            foreach (var dto in dtos)
            {
                dto.Email = allStaff.FirstOrDefault(s => s.Id == dto.Id).User.Email;
            }

            int count = dtos.Cast<VolunteerAdminReadEditDto>().Count();
            return dm.requiresCounts ? new JsonResult(new { result = dtos.Skip(dm.skip).Take(dm.take), count = count }) : new JsonResult(dtos);
        }

        //public JsonResult OnPostGetStaff([FromBody]Data crudRequest)
        //{
        //    IEnumerable DataSource = OrdersDetails.GetAllRecords();
        //    DataOperations operation = new DataOperations();
        //    if (crudRequest.Search != null && crudRequest.Search.Count > 0)
        //    {
        //        DataSource = operation.PerformSearching(DataSource, crudRequest.Search);  //Search
        //    }
        //    if (crudRequest.Sorte != null && crudRequest.Sorted.Count > 0) //Sorting
        //    {
        //        DataSource = operation.PerformSorting(DataSource, crudRequest.Sorted);
        //    }
        //    if (crudRequest.Where != null && crudRequest.Where.Count > 0) //Filtering
        //    {
        //        DataSource = operation.PerformFiltering(DataSource, crudRequest.Where, crudRequest.Where[0].Operator);
        //    }
        //    int count = DataSource.Cast<OrdersDetails>().Count();
        //    if (crudRequest.Skip != 0)
        //    {
        //        DataSource = operation.PerformSkip(DataSource, crudRequest.Skip);   //Paging
        //    }
        //    if (crudRequest.Take != 0)
        //    {
        //        DataSource = operation.PerformTake(DataSource, crudRequest.Take);
        //    }
        //    return crudRequest.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        //}

        public async Task OnPostSearch()
        {
            var volunteerDomainModels = await PrepareModel();
            Searcher searcher = new Searcher();
            volunteerDomainModels = searcher.FilterVolunteersBySearch(volunteerDomainModels, SearchedName, null);
            Staff = _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);

            ArchivedFilter = ArchivedFilter;
        }

        public async Task<IActionResult> OnPostChangeStatus(int volId, int status, bool archivedFilter)
        {
            var selectedStaff = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == volId);

            _context.Update(selectedStaff);

            selectedStaff.ApprovalStatus = (ApprovalStatus)status;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully changed {selectedStaff.FirstName} {selectedStaff.LastName} to {Enum.GetName(typeof(ApprovalStatus), status)}.", archivedFilter });
        }
        public async Task<IActionResult> OnPostDeleteStaff()
        {
            var selectedStaff = await _context.VolunteerProfiles
                .Include(p => p.Shifts)
                .FirstOrDefaultAsync(p => p.Id == SelectedStaffId);

            _context.Remove(selectedStaff);

            //foreach (Shift shift in Volunteer.Shifts)
            //{
            //    _context.Update(shift);
            //    shift.Volunteer = null;
            //    shift.CreateDescription();
            //}
            await _context.SaveChangesAsync();

            return RedirectToPage(new { statusMessage = $"You have successfully deleted {selectedStaff.FirstName} {selectedStaff.LastName} from staff." });
        }

        public async Task<IActionResult> OnPostAddNewStaff()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newStaffProfile = _mapper.Map<VolunteerProfile>(NewStaff);
            newStaffProfile.IsStaff = true;

            var user = new AppUser
            {
                UserName = NewStaff.Email,
                Email = NewStaff.Email,
                VolunteerProfile = newStaffProfile
            };

            var accountCreationResult = await _userManager.CreateAsync(user, NewStaff.Password);

            if (accountCreationResult.Succeeded)
            {
                return RedirectToPage(new { statusMessage = $"You have successfully added {newStaffProfile.FirstName} {newStaffProfile.LastName}" });
            }
            else
            {
                return RedirectToPage(new { statusMessage = $"Error: Something went wrong when adding the new staff member. Please try again or consult the admin." });
            }
        }

        private async Task<List<VolunteerProfile>> PrepareModel(string statusMessage = null, bool archiveFilter = false)
        {
            // get only staff
            var staffDomainModels = await _context.VolunteerProfiles.Include(p => p.Positions).Where(v => v != null && v.IsStaff).ToListAsync();
            Positions = _context.Positions.Where(p => !p.Deleted).OrderBy(p => p.Name).ToList();
            SearchedPositionId = Positions.FirstOrDefault(p => p.Name == "All").Id;

            var filteredStaff = new List<VolunteerProfile>();

            StatusMessage = statusMessage;

            ArchivedFilter = archiveFilter;

            // filter according to parameters
            foreach (var volunteer in staffDomainModels)
            {
                bool isActive = (volunteer.ApprovalStatus == ApprovalStatus.Approved);
                bool passeddeleted = (volunteer.ApprovalStatus == ApprovalStatus.Archived) == ArchivedFilter && ArchivedFilter;

                if (isActive || passeddeleted)
                {
                    filteredStaff.Add(volunteer);
                }
            }

            return filteredStaff;
        }
    }
}
