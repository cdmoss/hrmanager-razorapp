using MHFoodBank.Web.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public class CurrentDateFilter
    {
        public bool CheckIfShiftDateIsAfterToday(Shift shift)
        {
            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (shift is RecurringShift recurringShift)
            {
                string excludedDatesString = "";

                for (int i = 0; i < recurringShift.ConstituentShifts.Count(); i++)
                {
                    if (recurringShift.ConstituentShifts[i].StartDate >= currentDate)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            DateTime selectedShiftDate = recurringShift.ConstituentShifts[j].StartDate;
                            TimeSpan selectedShiftTime = recurringShift.StartTime;

                            DateTime combinedDateTime = new DateTime(
                                selectedShiftDate.Year,
                                selectedShiftDate.Month,
                                selectedShiftDate.Day,
                                selectedShiftTime.Hours,
                                selectedShiftTime.Minutes,
                                selectedShiftTime.Seconds);

                            excludedDatesString += $"\\nEXDATE:{combinedDateTime.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";
                        }

                        recurringShift.RecurrenceRule += excludedDatesString;
                        return true;
                    }
                }
            }
            else
            {
                if (shift.StartDate >= currentDate)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
