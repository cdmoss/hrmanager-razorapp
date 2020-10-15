using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Areas.Admin.Pages.Shared;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MHFoodBank.Common.Dtos;
using AutoMapper;
using MHFoodBank.Common;
using Syncfusion.EJ2.Base;
using MHFoodBank.Web.Services;

namespace MHFoodBank.Web.Areas.Admin.Pages
{
    [Authorize(Roles = "Staff, Admin")]
    [BindProperties]
    public class AdminCalendar : AdminPageModel
    {
        private readonly IAdminCalendarService _calendarService;

        #region pagemodel properties
        // for choosing a volunteer when editing/adding a shift
        public List<Position> PositionsWithAll { get; set; }
        public List<Position> PositionsWithoutAll { get; set; }
        public string[] ResourceNames { get; set; } = new string[] { "Positions" };
        public List<VolunteerMinimalDto> Volunteers { get; set; }
        public string SearchedName { get; set; }
        public int SearchedPositionId { get; set; }
        // position that was selected in the edit/delete position window
        public string SelectedPositionName { get; set; }
        public string NewSelectedPositionName { get; set; }
        public string NewSelectedPositionColor { get; set; }
        public string NewPositionColor { get; set; }
        public string NewPositionName { get; set; }
        // give user feedback after action
        public string StatusMessage { get; set; }
        [BindProperty(Name = nameof(ShiftAmounts))] public Dictionary<int, int> ShiftAmounts { get; set; }
        #endregion

        public AdminCalendar(IAdminCalendarService calendarService, string currentPage = "Scheduling") : base(currentPage)
        {
            _calendarService.ReportAlertCount();
            _calendarService = calendarService;
        }

        public async Task OnGet()
        {
            await PrepareModel(null);
        }

        private async Task PrepareModel(string statusMessage)
        {
            Volunteers = await _calendarService.GetApprovedVolunteerMinimals();

            // get positions
            PositionsWithoutAll = await _calendarService.GetPositionsWithoutAll();
            PositionsWithAll = await _calendarService.GetPositonsWithAll();
            SearchedPositionId = PositionsWithAll.FirstOrDefault(p => p.Name == "All").Id;

            ShiftAmounts = _calendarService.InitializeShiftAmounts(PositionsWithoutAll);

            // update status message
            StatusMessage = statusMessage;
        }

        public JsonResult OnPostGetShifts()
        {
            return new JsonResult(_calendarService.GetShifts());
        }

        public async Task<JsonResult> OnPostHandleShiftCrudRequest([FromBody] CRUDModel<ShiftReadEditDto> crudRequest)
        {
            string resultMessage = "";
            if (crudRequest != null)
            {
                if (crudRequest.Action == "insert" || (crudRequest.Action == "batch" && crudRequest.Added.Count > 0)) // this block of code will execute while inserting the appointments
                {
                    var newShiftDto = (crudRequest.Action == "insert") ? crudRequest.Value : crudRequest.Added[0];
                    resultMessage = await _calendarService.CreateShift(newShiftDto, crudRequest.Params);
                }
                // when a shift is deleted from a recurring shift it enters this block
                if (crudRequest.Action == "update" || (crudRequest.Action == "batch" && crudRequest.Changed.Count > 0)) // this block of code will execute while updating the appointment
                {
                    var newShiftDto = (crudRequest.Action == "update") ? crudRequest.Value : crudRequest.Changed[0];
                    await _calendarService.UpdateShift(newShiftDto);
                }
                if (crudRequest.Action == "remove" || (crudRequest.Action == "batch" && crudRequest.Deleted.Count > 0)) // this block of code will execute while removing the appointment
                {
                    var newShiftDtos = (crudRequest.Action == "remove") ? new List<ShiftReadEditDto>() { crudRequest.Value } : crudRequest.Deleted;
                    await _calendarService.DeleteShifts(newShiftDtos);
                };
            }
            else
            {
                resultMessage = "There was no data provided for the new shift";
            }

            var result = new List<object>();

            result.Union(await _calendarService.GetShifts());
            result.Add(resultMessage);

            return new JsonResult(result);
        }

        public async Task OnPostSearch()
        {
            //store in local variable so it doesn't get overwritten by prepare model
            int searchedPosId = SearchedPositionId;
            await PrepareModel(null);

            var searcher = new Searcher();
            var searchedPosition = _calendarService.GetPositionById(searchedPosId);
            //Shifts = searcher.FilterShiftsBySearch(Shifts, SearchedName, searchedPosition);
        }

        public async Task<IActionResult> OnPostAddPosition(string newPosition, string newPositionColor)
        {
            string resultStatus = await _calendarService.CreatePosition(newPosition, newPositionColor);
            return RedirectToPage(new { statusMessage = resultStatus });
        }

        public async Task<IActionResult> OnPostEditPosition()
        {
            string resultStatus = await _calendarService.UpdatePosition(SelectedPositionName, NewSelectedPositionName, NewSelectedPositionColor);
            return RedirectToPage(new { statusMessage = resultStatus });
        }

        public async Task<IActionResult> OnPostRemovePosition()
        {
            string resultStatus = await _calendarService.DeletePosition(SelectedPositionName);
            return RedirectToPage(new { statusMessage = resultStatus });
        }
    }
}