using MHFoodBank.Web.Data;
using MHFoodBank.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using Xunit;
using System;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using System.Collections.Generic;

namespace MHFoodbank.Test
{
    public class AdminCalendarTests
    {
        // TODO: seed database for testing
        private AdminCalendarService GetCalendarService()
        {
            var testServiceProvider = new TestServiceProvider();
            var mapper = testServiceProvider.GetMapper();
            var config = testServiceProvider.GetConfig();
            var contextOptionsBuilder = new DbContextOptionsBuilder<FoodBankContext>()
                .UseMySql(config.GetConnectionString("MainDevConnection"));
            var context = new FoodBankContext(contextOptionsBuilder.Options);
            var logger = new NullLoggerFactory().CreateLogger<ReminderManager>();
            var emailSender = new TestEmailSender();
            var reminderManager = new ReminderManager(context, logger, emailSender);
            return new AdminCalendarService(context, mapper, reminderManager);
        }

        [Fact]
        public async Task CreateShift_ReportsErrorAndReturns_IfIncompleteShift()
        {
            var calendarService = GetCalendarService();
            var newShiftNoPos = new ShiftReadEditDto() { StartTime = DateTime.Now.AddHours(6), EndTime = DateTime.Now.AddHours(12) };
            var newShiftNoStart = new ShiftReadEditDto() { StartTime = DateTime.Now.AddHours(6), PositionId = 3 };
            var newShiftNoEnd = new ShiftReadEditDto() { EndTime = DateTime.Now.AddHours(12), PositionId = 3};
            var insertParams = new Dictionary<string, object>();
            string resultNoPos = await calendarService.CreateShift(newShiftNoPos, insertParams);
            string resultNoStart = await calendarService.CreateShift(newShiftNoStart, insertParams);
            string resultNoEnd = await calendarService.CreateShift(newShiftNoEnd, insertParams);
            int dbShiftsAmount = (await calendarService.GetShifts()).Count;
            Assert.Equal("The provided data for the new shift was missing either a StartTime, Endtime or Position", resultNoPos);
            Assert.Equal("The provided data for the new shift was missing either a StartTime, Endtime or Position", resultNoStart);
            Assert.Equal("The provided data for the new shift was missing either a StartTime, Endtime or Position", resultNoEnd);
            Assert.Equal(0, dbShiftsAmount);
        }

        [Fact]
        public async Task CreateShift_ReportsErrorAndReturns_IfStartTimeIsMissing()
        {
            var calendarService = GetCalendarService();
            var newShift = new ShiftReadEditDto() { StartTime = DateTime.Now.AddHours(6), EndTime = DateTime.Now.AddHours(12), };
            var insertParams = new Dictionary<string, object>();
            string result = await calendarService.CreateShift(newShift, insertParams);
            Assert.Equal("The provided data for the new shift was missing either a StartTime, Endtime or Position", result);
        }

        [Fact]
        public async Task AdminCalendar_TestUpdateShift()
        {

        }

        [Fact]
        public async Task AdminCalendar_TestRemoveShift()
        {

        }

        [Fact]
        public async Task AdminCalendar_TestCheckIfShiftBeingRemoved()
        {

        }

        [Fact]
        public async Task AdminCalendar_UpdateShiftProperties()
        {

        }

        [Fact]
        public async Task AdminCalendar_TestAddPosition()
        {

        }

        [Fact]
        public async Task AdminCalendar_TestEditPosition()
        {

        }

        [Fact]
        public async Task AdminCalendar_TestRemovePosition()
        {

        }
    }
}
