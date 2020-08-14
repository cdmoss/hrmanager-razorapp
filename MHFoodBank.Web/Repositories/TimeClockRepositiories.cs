using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Common;
using MHFoodBank.Web.Data;

namespace MHFoodBank.Web.Repositories
{
    public interface IClockedTimeRepo
    {
        Task<List<ClockedTime>> GetAllClockedTimes();
        Task<List<ClockedTime>> GetClockedTimesByVolunteer(int volunteerId);
        Task<ClockedTime> GetClockedTimeById(int id);
        Task<PunchClockResult> PunchClock(int userId, int position);
        Task<bool> UpdateClockedTime(int id);
        Task<bool> DeleteClockedTime(int id);
    }

    public class ClockedTimeRepo : IClockedTimeRepo
    {
        private readonly FoodBankContext _context;

        public ClockedTimeRepo(FoodBankContext context)
        {
            _context = context;
        }

        public async Task<PunchClockResult> PunchClock(int userId, int position)
        {
            try
            {
                var volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.UserID == userId);

                var existingClockIn = await _context.ClockedTime.FirstOrDefaultAsync(ct => ct.Volunteer.UserID == userId &&
                                                                             ct.EndTime == null);
                if (existingClockIn != null)
                {
                    _context.Update(existingClockIn);
                    existingClockIn.EndTime = DateTime.UtcNow.AddHours(-6);
                    await _context.SaveChangesAsync();

                    return new PunchClockResult
                    {
                        Message = $"Goodbye {existingClockIn.Volunteer.FirstName}! You have successfully clocked out.",
                        Success = true
                    };
                }
                else
                {
                    ClockedTime newClockIn = new ClockedTime()
                    {
                        Volunteer = volunteer,
                        StartTime = DateTime.UtcNow.AddHours(-6),
                        Position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == position)
                    };
                    await _context.AddAsync(newClockIn);
                    await _context.SaveChangesAsync();

                    return new PunchClockResult
                    {
                        Message = $"Welcome {newClockIn.Volunteer.FirstName}! You have successfully clocked in.",
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new PunchClockResult
                {
                    Message = $"Something went wrong, please try again.",
                    Success = false
                };
            }
        }

        public Task<bool> DeleteClockedTime(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClockedTime>> GetAllClockedTimes()
        {
            throw new NotImplementedException();
        }

        public Task<ClockedTime> GetClockedTimeById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClockedTime>> GetClockedTimesByVolunteer(int volunteerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateClockedTime(int id)
        {
            throw new NotImplementedException();
        }
    }
}
