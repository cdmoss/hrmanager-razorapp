using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Common.Services;
using MHFoodBank.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public interface IAdminCalendarService : IAlertCountReporter
    {
        Task<List<ShiftReadEditDto>> GetShifts();
        Task<string> CreateShift(ShiftReadEditDto shiftDto, IDictionary<string, object> insertParams);
        Task<string> UpdateShift(ShiftReadEditDto shiftDto);
        Task<string> DeleteShifts(List<ShiftReadEditDto> shiftDto);
        Task<string> CreatePosition(string newPositionName, string newPositionColor);
        Task<string> UpdatePosition(string selectedPositionName, string newNameForPosition, string newColorForPosition);
        Task<string> DeletePosition(string selectedPositionName);
        Task<List<VolunteerMinimalDto>> GetApprovedVolunteerMinimals();
        Task<List<Position>> GetPositonsWithAll();
        Task<List<Position>> GetPositionsWithoutAll();
        Task<Position> GetPositionById(int searchedPosId);

        Dictionary<int, int> InitializeShiftAmounts(List<Position> positions);
    }
    
    public class AdminCalendarService : IAdminCalendarService
    {
        private readonly FoodBankContext _context;
        private readonly IReminderManager _reminderManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminCalendarService> _logger;

        public AdminCalendarService(FoodBankContext context, IMapper mapper, IReminderManager reminderManager)
        {
            _context = context;
            _mapper = mapper;
            _reminderManager = reminderManager;
        }

        // interface methods
        public async Task<int> ReportAlertCount()
        {
            return await _context.Alerts.CountAsync(a => !a.Read);
        }

        public async Task<List<ShiftReadEditDto>> GetShifts()
        {
            var shifts = await _context.Shifts.ToListAsync();
            return _mapper.Map<List<ShiftReadEditDto>>(shifts);
        }

        public async Task<string> CreateShift(ShiftReadEditDto newShiftDto, IDictionary<string, object> insertParams)
        {
            bool incompleteShift =
                newShiftDto.StartTime == null ||
                newShiftDto.EndTime == null ||
                ((newShiftDto.PositionId == null || newShiftDto.PositionId == 0) && newShiftDto.PositionWorked == null);

            if (incompleteShift)
            {
                _logger.LogError("The provided data for the new shift was missing either a StartTime, Endtime or Position");
                return "The provided data for the new shift was missing either a StartTime, Endtime or Position";
            }

            // this must be set manually to prevent errors when adding it to the context
            newShiftDto.Id = 0;

            // the datetime data from syncfusion's calendar is consistently 6 hours behind, 
            // so it is corrected here
            newShiftDto.StartTime = newShiftDto.StartTime.AddHours(-6);
            newShiftDto.EndTime = newShiftDto.EndTime.AddHours(-6);

            var newShift = _mapper.Map<Shift>(newShiftDto);

            // check if the the request requires multiple shifts to be created
            bool multipleShifts = false;
            if (insertParams.ContainsKey("multiShifts"))
            {
                multipleShifts = Convert.ToBoolean(insertParams["multiShifts"]);
            }

            if (multipleShifts)
            {
                await CreateMultipleShifts(newShift, insertParams);
                return "A new shift has successfully been created";
            }
            else
            {
                await CreateSingleShift(newShift);
                return "Multiple shifts has successfully been created.";
            }
        }

        private async Task CreateSingleShift(Shift newShift)
        {
            var chosenPosition = newShift.Position;
            var chosenVolunteer = newShift.VolunteerProfileId != null || newShift.VolunteerProfileId > 0 ? _context.VolunteerProfiles
                .Where(p => p.Id == newShift.VolunteerProfileId).FirstOrDefault() : null;

            newShift.Subject = newShift.VolunteerProfileId == null ?
                $"Open - {chosenPosition.Name}" :
                $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

            _context.Shifts.Add(newShift);

            await _context.SaveChangesAsync();
        }

        private async Task CreateMultipleShifts(Shift newShift, IDictionary<string, object> positionParams)
        {
            var positions = await _context.Positions.Where(p => p.Name != "All").ToListAsync();

            // for each position in db, check if it was included in the request params,
            // if so, create the corresponding amount of shifts requested for that position
            foreach (var pos in positions)
            {
                int positionAmount = Convert.ToInt32(positionParams[$"{pos.Name}-amount"]);
                for (int i = 0; i < positionAmount; i++)
                {
                    newShift.Id = 0;
                    newShift.Position = pos;

                    var chosenPosition = pos;
                    var chosenVolunteer = newShift.VolunteerProfileId != null || newShift.VolunteerProfileId > 0 ? 
                        await _context.VolunteerProfiles
                            .Where(p => p.Id == newShift.VolunteerProfileId).FirstOrDefaultAsync() :
                        null;

                    newShift.Subject = newShift.VolunteerProfileId == null ?
                        $"Open - {chosenPosition.Name}" :
                        $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                    _context.Shifts.Add(newShift);

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<string> UpdateShift(ShiftReadEditDto newShiftDto)
        {
            var shiftBeingUpdated = _context.Shifts.FirstOrDefault(c => c.Id == Convert.ToInt32(newShiftDto.Id));

            if (shiftBeingUpdated == null)
            {
                return "The shift to be updated could not be found.";
            }

            // this method cancels the reminder for the shift being removed as a side effect.
            // The side effect is necessary to prevent having to calculate the exDate list more than once
            bool? shiftIsBeingRemovedFromRecurringSet = await CheckIfShiftIsBeingRemovedFromRecurringSet(newShiftDto, shiftBeingUpdated);

            if (shiftIsBeingRemovedFromRecurringSet == null)
            {
                return "The shift to be updated could not be found.";
            }

            // this runs in the standard case, a shift model's data is simply being changed
            if (shiftIsBeingRemovedFromRecurringSet == false)
            {
                // cancel old reminder if old version of shift had a volunteer
                if (shiftBeingUpdated.VolunteerProfileId != null)
                {
                    _reminderManager.CancelReminder(shiftBeingUpdated);
                }

                bool initialUpdateSuccessful = UpdateShiftProperties(newShiftDto, shiftBeingUpdated);

                if (!initialUpdateSuccessful)
                {
                    return "The data provided to update the shift with was incomplete: either the Startime or EndTime was null.";
                }

                var chosenPosition = _context.Positions.FirstOrDefault(p => p.Id == shiftBeingUpdated.PositionId);
                var chosenVolunteer = shiftBeingUpdated.VolunteerProfileId != null ? _context.VolunteerProfiles
                    .FirstOrDefault(p => p.Id == shiftBeingUpdated.VolunteerProfileId) : null;

                shiftBeingUpdated.Subject = shiftBeingUpdated.VolunteerProfileId == null ?
                $"Open - {chosenPosition.Name}" :
                    $"{chosenVolunteer.FirstName} {chosenVolunteer.LastName} - {chosenPosition.Name}";

                await _context.SaveChangesAsync();

                //create new reminder if new version of shift has volunteer and if this isn't a request to delete a child shift from its recurring set
                if (shiftBeingUpdated.VolunteerProfileId != null || shiftBeingUpdated.VolunteerProfileId > 0)
                {
                    bool reminderScheduledSuccessfully = _reminderManager.ScheduleReminder(shiftBeingUpdated);
                    if (!reminderScheduledSuccessfully)
                    {
                        return "Something went wrong when trying to schedule the reminder for the updated shift.";
                    }
                }

                return "The selected shift was successfully updated.";
            }

            return "The shift was successfully removed from the recurring set.";
        }

        public async Task DeleteShifts(List<ShiftReadEditDto> shiftDtos)
        {
            foreach (var shift in shiftDtos)
            {
                var newShift = await _context.Shifts.Where(c => c.Id == shift.Id).FirstOrDefaultAsync();
                if (newShift != null)
                {
                    _reminderManager.CancelReminder(newShift);
                    _context.Shifts.Remove(newShift);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<string> CreatePosition(string newPositionName, string newPositionColor)
        {
            // get entered name and check if a position with that name exists

            bool positionAlreadyExists = _context.Positions.Any(p => string.Equals(newPositionName, p.Name));
            bool noPositionNameEntered = string.IsNullOrWhiteSpace(newPositionColor);

            if (positionAlreadyExists)
            {
                return $"Error: A position with that name already exists.";
            }
            else if (noPositionNameEntered)
            {
                return $"Error: You must enter a name for the position.";
            }

            var newPosition = new Position { Name = newPositionName, Color = newPositionColor};
            await _context.Positions.AddAsync(newPosition);
            await _context.SaveChangesAsync();

            return $"You successfully added {newPosition.Name} to the list of positions.";
        }

        public async Task<string> UpdatePosition(string selectedPositionName, string newNameForPosition, string newColorForPosition)
        {
            bool positionAlreadyExists = await _context.Positions.AnyAsync(p => string.Equals(newNameForPosition, p.Name));
            bool noPositionNameEntered = string.IsNullOrWhiteSpace(newNameForPosition);

            if (positionAlreadyExists)
            {
                return $"Error: A position with that name already exists.";
            }
            else if (noPositionNameEntered)
            {
                return $"Error: You must enter a name for the position.";
            }

            var selectedPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Name == selectedPositionName);

            if (selectedPosition != null)
            {
                string originalName = selectedPosition.Name;

                _context.Update(selectedPosition);

                selectedPosition.Name = newNameForPosition;
                selectedPosition.Color = newColorForPosition;

                await _context.SaveChangesAsync();
                return $"You successfully updated {originalName} to {selectedPosition.Name}.";
            }
            return $"Error: You must select a position.";
        }

        public async Task<string> DeletePosition(string selectedPositionName)
        {
            var selectedPosition = _context.Positions.FirstOrDefault(p => p.Name == selectedPositionName);

            if (selectedPosition != null)
            {
                var positionVolunteers = await _context.PositionVolunteers.Where(s => s.Position == selectedPosition).ToListAsync();
                var shiftsWithPosition = await _context.Shifts.Where(s => s.Position == selectedPosition).ToListAsync();

                foreach (var pv in positionVolunteers)
                {
                    pv.Position = null;
                }

                foreach (var shift in shiftsWithPosition)
                {
                    shift.Position = null;
                }

                selectedPosition.Deleted = true;
                await _context.SaveChangesAsync();
                return $"You successfully removed {selectedPosition.Name} from the list of positions.";
            }
            return $"Error: You must select a position.";
        }

        public async Task<List<VolunteerMinimalDto>> GetApprovedVolunteerMinimals()
        {
            var volunteerDomainModels = await _context.VolunteerProfiles
                .Include(p => p.Positions)
                .Where(v => v.ApprovalStatus == ApprovalStatus.Approved).ToListAsync();
            return _mapper.Map<List<VolunteerMinimalDto>>(volunteerDomainModels);
        }

        public async Task<List<Position>> GetPositonsWithAll()
        {
            return await _context.Positions.Where(p => !p.Deleted).OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<List<Position>> GetPositionsWithoutAll()
        {
            return await _context.Positions.Where(p => !p.Deleted && p.Name != "All").OrderBy(p => p.Name).ToListAsync();
        }
        public async Task<Position> GetPositionById(int searchedPosId)
        {
            return await _context.Positions.FirstOrDefaultAsync(p => p.Id == searchedPosId);
        }

        public Dictionary<int, int> InitializeShiftAmounts(List<Position> positions)
        {
            var shiftAmounts = new Dictionary<int, int>();

            foreach (var position in positions)
            {
                shiftAmounts.Add(position.Id, 0);
            }

            return shiftAmounts;
        }

        // private methods exclusive to this class
        // this method cancels the reminder for the shift being removed as a side effect
        private async Task<bool?> CheckIfShiftIsBeingRemovedFromRecurringSet(ShiftReadEditDto newShiftDto, Shift shiftBeingUpdated)
        {
            // determine if the shift selected for change is a shift that's being removed from its recurring set.
            // this is done by comparing the amount of exdates in the old recshift to the amount in the new recshift.
            // if the new shift has more exdates than the old shift, a child shift is being removed
            var oldExDates = shiftBeingUpdated.RecurrenceException == null ?
                new List<DateTime>() :
                RecurrenceHelper.ConvertExDateStringToDateTimes(shiftBeingUpdated.RecurrenceException);

            var newExDates = newShiftDto.RecurrenceException == null ?
                new List<DateTime>() :
                RecurrenceHelper.ConvertExDateStringToDateTimes(newShiftDto.RecurrenceException);

            bool shiftIsBeingRemovedFromRecurringSet = newExDates.Count() > oldExDates.Count();

            // if shift is being removed, pass in the date of the last shift excluded when cancelling reminder and update exdate list
            try
            {
                if (shiftIsBeingRemovedFromRecurringSet)
                {
                    var newExDate = newExDates.Last();
                    _reminderManager.CancelReminder(shiftBeingUpdated, newExDate);

                    shiftBeingUpdated.RecurrenceException = newShiftDto.RecurrenceException;
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong when trying to cancel a reminder for a shift being removed from its recurring set. \nError Message:\n{ex.Message}");
                return null;
            }
        }

        private bool UpdateShiftProperties(ShiftReadEditDto newShiftDto, Shift shiftBeingUpdated)
        {
            if (newShiftDto.StartTime == null || newShiftDto.EndTime == null)
            {
                return false;
            }
            shiftBeingUpdated.StartTime = newShiftDto.StartTime.AddHours(-6);
            shiftBeingUpdated.EndTime = newShiftDto.EndTime.AddHours(-6);
            shiftBeingUpdated.VolunteerProfileId = newShiftDto.VolunteerProfileId;
            shiftBeingUpdated.PositionId = newShiftDto.PositionId;
            shiftBeingUpdated.IsAllDay = newShiftDto.IsAllDay;
            shiftBeingUpdated.RecurrenceRule = newShiftDto.RecurrenceRule;
            shiftBeingUpdated.RecurrenceID = newShiftDto.RecurrenceID;
            shiftBeingUpdated.RecurrenceException = newShiftDto.RecurrenceException;
            shiftBeingUpdated.IsRecurrence = !string.IsNullOrWhiteSpace(shiftBeingUpdated.RecurrenceRule);

            return true;
        }
    }
}