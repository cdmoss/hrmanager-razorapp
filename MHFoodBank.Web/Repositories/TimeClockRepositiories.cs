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
        Task<OperationResponse<object>> PunchClock(int userId, int position);
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

        public async Task<OperationResponse<object>> PunchClock(int userId, int position)
        {
            try
            {
                var volunteer = await _context.VolunteerProfiles.FirstOrDefaultAsync(v => v.UserID == userId);

                var clockIn = await _context.ClockedTime.FirstOrDefaultAsync(ct => ct.VolunteerProfile.UserID == userId &&
                                                                             ct.EndTime == null);
                if (clockIn != null)
                {
                    _context.Update(clockIn);
                    clockIn.EndTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return new OperationResponse<object>
                    {
                        Message = $"Goodbye {clockIn.VolunteerProfile.FirstName}! You have successfully clocked out.",
                        Success = true
                    };
                }
                else
                {
                    ClockedTime clock = new ClockedTime()
                    {
                        VolunteerProfile = volunteer,
                        StartTime = DateTime.UtcNow,
                        Position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == position)
                    };
                    await _context.AddAsync(clock);
                    await _context.SaveChangesAsync();

                    return new OperationResponse<object>
                    {
                        Message = $"Welcome {clockIn.VolunteerProfile.FirstName}! You have successfully clocked in.",
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new OperationResponse<object>
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
