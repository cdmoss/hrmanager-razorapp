using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkplaceAdministrator.Api.Services
{
    public interface INotificationService
    {
        Task<bool> ScheduleShiftReminder();
        Task<bool> SendAvailableShiftsNotification();
    }

    public class NotificationService : INotificationService
    {
        public Task<bool> ScheduleShiftReminder()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendAvailableShiftsNotification()
        {
            throw new NotImplementedException();
        }
    }
}
