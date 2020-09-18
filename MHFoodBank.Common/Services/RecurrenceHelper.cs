using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MHFoodBank.Common.Services
{
    public class RecurrenceHelper
    {
        #region Internal Fields

        //internal static Dictionary<string, List<string>> ICalTimeZones = AddTimeZones();
        internal static List<string> valueList = new List<string>();
        //internal static Dictionary<string, RecurrenceProperties> RecPropertiesDict = new Dictionary<string, RecurrenceProperties>();
        internal static string COUNT;
        internal static string RECCOUNT;
        internal static string DAILY;
        internal static string WEEKLY;
        internal static string MONTHLY;
        internal static string YEARLY;
        internal static string INTERVAL;
        internal static string INTERVALCOUNT;
        internal static string BYSETPOS;
        internal static string BYSETPOSCOUNT;
        internal static string BYDAY;
        internal static string BYDAYVALUE;
        internal static string BYMONTHDAY;
        internal static string BYMONTHDAYCOUNT;
        internal static string BYMONTH;
        internal static string BYMONTHCOUNT;
        internal static int BYDAYPOSITION;
        internal static int BYMONTHDAYPOSITION;
        internal static int WEEKLYBYDAYPOS;
        internal static string WEEKLYBYDAY;
        internal static string EXDATE;
        internal static List<DateTime> exDateList = new List<DateTime>();
        #endregion

        #region Methods

        public static IEnumerable<DateTime> GetRecurrenceDateTimeCollection(string RRule, DateTime RecStartDate)
        {
            var RecDateCollection = new ObservableCollection<DateTime>();
            DateTime startDate = RecStartDate;
            var ruleSeperator = new[] { '=', ';', ',' };
            var weeklySeperator = new[] { ';' };
            string[] ruleArray = RRule.Split(ruleSeperator);
            FindKeyIndex(ruleArray);
            string[] weeklyRule = RRule.Split(weeklySeperator);
            FindWeeklyRule(weeklyRule);
            FindExdateList(weeklyRule);
            if (ruleArray.Length != 0 && RRule != "")
            {
                DateTime addDate = startDate;
                int recCount = int.Parse(RECCOUNT);

                #region DAILY
                if (DAILY == "DAILY")
                {

                    if ((ruleArray.Length > 4 && INTERVAL == "INTERVAL") || ruleArray.Length == 4)
                    {
                        int DyDayGap = ruleArray.Length == 4 ? 1 : int.Parse(INTERVALCOUNT);
                        for (int i = 0; i < recCount; i++)
                        {
                            RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddDays(DyDayGap);
                        }
                    }
                    else if (ruleArray.Length > 4 && BYDAY == "BYDAY")
                    {
                        while (RecDateCollection.Count < recCount)
                        {
                            if (addDate.DayOfWeek != DayOfWeek.Sunday && addDate.DayOfWeek != DayOfWeek.Saturday)
                            {

                                RecDateCollection.Add(addDate.Date);
                            }
                            addDate = addDate.AddDays(1);
                        }
                    }
                }
                #endregion

                #region WEEKLY
                else if (WEEKLY == "WEEKLY")
                {
                    int WyWeekGap = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? int.Parse(INTERVALCOUNT) : 1;
                    bool isweeklyselected = weeklyRule[WEEKLYBYDAYPOS].Length > 6;
                    while (RecDateCollection.Count < recCount && isweeklyselected)
                    {
                        switch (addDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("SU") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Monday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("MO") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Tuesday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("TU") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("WE") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("TH") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("FR") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    if (weeklyRule[WEEKLYBYDAYPOS].Contains("SA") && RecDateCollection.Count < recCount)
                                    {
                                        RecDateCollection.Add(addDate.Date);
                                    }
                                    break;
                                }
                        }
                        addDate = addDate.DayOfWeek == DayOfWeek.Saturday ? addDate.AddDays(((WyWeekGap - 1) * 7) + 1) : addDate.AddDays(1);
                    }
                }
                #endregion

                #region MONTHLY
                else if (MONTHLY == "MONTHLY")
                {
                    int MyMonthGap = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? int.Parse(INTERVALCOUNT) : 1;
                    int position = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? 6 : BYMONTHDAYPOSITION;
                    if (BYMONTHDAY == "BYMONTHDAY")
                    {
                        int monthDate = int.Parse(BYMONTHDAYCOUNT);
                        if (monthDate < 30)
                        {
                            int currDate = int.Parse(startDate.Day.ToString());
                            var temp = new DateTime(addDate.Year, addDate.Month, monthDate);
                            addDate = monthDate < currDate ? temp.AddMonths(1) : temp;
                            for (int i = 0; i < recCount; i++)
                            {
                                if (addDate.Month == 2 && monthDate > 28)
                                {
                                    addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, 2));
                                    RecDateCollection.Add(addDate.Date);
                                    addDate = addDate.AddMonths(MyMonthGap);
                                    addDate = new DateTime(addDate.Year, addDate.Month, monthDate);
                                }
                                else
                                {
                                    RecDateCollection.Add(addDate.Date);
                                    addDate = addDate.AddMonths(MyMonthGap);
                                }
                            }
                        }
                        else
                        {
                            addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, addDate.Month));
                            for (int i = 0; i < recCount; i++)
                            {
                                RecDateCollection.Add(addDate.Date);
                                addDate = addDate.AddMonths(MyMonthGap);
                                addDate = new DateTime(addDate.Year, addDate.Month, DateTime.DaysInMonth(addDate.Year, addDate.Month));
                            }
                        }

                    }
                    else if (BYDAY == "BYDAY")
                    {
                        while (RecDateCollection.Count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, addDate.Month, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = GetWeekDay(BYDAYVALUE) - 1;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = int.Parse(BYSETPOSCOUNT) - 1;
                            }
                            else
                            {
                                nthWeek = int.Parse(BYSETPOSCOUNT);
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddMonths(1);
                                continue;
                            }

                            RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddMonths(MyMonthGap);
                        }
                    }
                }
                #endregion

                #region YEARLY
                else if (YEARLY == "YEARLY")
                {
                    int YyYearGap = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? int.Parse(INTERVALCOUNT) : 1;
                    int position = ruleArray.Length > 4 && INTERVAL == "INTERVAL" ? 6 : BYMONTHDAYPOSITION;
                    if (BYMONTHDAY == "BYMONTHDAY")
                    {
                        int monthIndex = int.Parse(BYMONTHCOUNT);
                        int dayIndex = int.Parse(BYMONTHDAYCOUNT);
                        if (monthIndex > 0 && monthIndex <= 12)
                        {
                            int bound = DateTime.DaysInMonth(addDate.Year, monthIndex);
                            if (bound >= dayIndex)
                            {
                                var specificDate = new DateTime(addDate.Year, monthIndex, dayIndex);
                                if (specificDate.Date < addDate.Date)
                                {
                                    addDate = specificDate;
                                    addDate = addDate.AddYears(1);

                                }
                                else
                                {
                                    addDate = specificDate;
                                }

                                for (int i = 0; i < recCount; i++)
                                {


                                    RecDateCollection.Add(addDate.Date);
                                    addDate = addDate.AddYears(YyYearGap);
                                }
                            }
                        }
                    }
                    else if (BYDAY == "BYDAY")
                    {
                        int monthIndex = int.Parse(BYMONTHCOUNT);
                        while (RecDateCollection.Count < recCount)
                        {
                            var monthStart = new DateTime(addDate.Year, monthIndex, 1);
                            DateTime weekStartDate = monthStart.AddDays(-(int)(monthStart.DayOfWeek));
                            var monthStartWeekday = (int)(monthStart.DayOfWeek);
                            int nthweekDay = GetWeekDay(BYDAYVALUE) - 1;
                            int nthWeek;
                            if (monthStartWeekday <= nthweekDay)
                            {
                                nthWeek = int.Parse(BYSETPOSCOUNT) - 1;
                            }
                            else
                            {
                                nthWeek = int.Parse(BYSETPOSCOUNT);
                            }
                            addDate = weekStartDate.AddDays((nthWeek) * 7);
                            addDate = addDate.AddDays(nthweekDay);
                            if (addDate.CompareTo(startDate) < 0)
                            {
                                addDate = addDate.AddYears(1);
                                continue;
                            }

                            RecDateCollection.Add(addDate.Date);
                            addDate = addDate.AddYears(YyYearGap);

                        }
                    }
                }
                #endregion
            }
            return RecDateCollection.Except(exDateList).ToList();
        }

        public static List<DateTime> FindExcludedDates(string rrule)
        {
            string[] weeklyRule = rrule.Split(';');
            FindExdateList(weeklyRule);

            var exList = new List<DateTime>();
            foreach (var date in exDateList)
            {
                exList.Add(date);
            }

            return exList;
        }

        public static string AddToExcludedDates(string rrule, DateTime exDate)
        {
            rrule += $"\\nEXDATE:{exDate.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}Z";
            return rrule;
        }

        private static void FindExdateList(string[] weeklyRule)
        {
            for (int i = 0; i < weeklyRule.Length; i++)
            {
                if (weeklyRule[i].Contains("EXDATE"))
                {
                    EXDATE = weeklyRule[i];
                    var _rule = weeklyRule[i].Split('=');
                    if (_rule[0] == "EXDATE")
                    {
                        var exDates = _rule[1].Split(',');
                        for (var j = 0; j < exDates.Length; j++)
                        {
                            exDateList.Add(DateTime.ParseExact(exDates[j], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        }
                    }
                    break;
                }
            }
        }

        private static int GetWeekDay(string weekDay)
        {
            switch (weekDay)
            {
                case "SU":
                    {
                        return 1;
                    }
                case "MO":
                    {
                        return 2;
                    }
                case "TU":
                    {
                        return 3;
                    }
                case "WE":
                    {
                        return 4;
                    }
                case "TH":
                    {
                        return 5;
                    }
                case "FR":
                    {
                        return 6;
                    }
                case "SA":
                    {
                        return 7;
                    }
            }
            return 8;
        }

        private static void FindWeeklyRule(string[] weeklyRule)
        {
            for (int i = 0; i < weeklyRule.Length; i++)
            {
                if (weeklyRule[i].Contains("BYDAY"))
                {
                    WEEKLYBYDAY = weeklyRule[i];
                    WEEKLYBYDAYPOS = i;
                    break;
                }
            }
        }

        private static void FindKeyIndex(string[] ruleArray)
        {
            RECCOUNT = "";
            DAILY = "";
            WEEKLY = "";
            MONTHLY = "";
            YEARLY = "";
            BYSETPOS = "";
            BYSETPOSCOUNT = "";
            INTERVAL = "";
            INTERVALCOUNT = "";
            COUNT = "";
            BYDAY = "";
            BYDAYVALUE = "";
            BYMONTHDAY = "";
            BYMONTHDAYCOUNT = "";
            BYMONTH = "";
            BYMONTHCOUNT = "";
            WEEKLYBYDAY = "";

            for (int i = 0; i < ruleArray.Length; i++)
            {
                if (ruleArray[i].Contains("COUNT"))
                {
                    COUNT = ruleArray[i];
                    RECCOUNT = ruleArray[i + 1];

                }
                if (ruleArray[i].Contains("DAILY"))
                {
                    DAILY = ruleArray[i];
                }

                if (ruleArray[i].Contains("WEEKLY"))
                {
                    WEEKLY = ruleArray[i];
                }
                if (ruleArray[i].Contains("INTERVAL"))
                {
                    INTERVAL = ruleArray[i];
                    INTERVALCOUNT = ruleArray[i + 1];
                }
                if (ruleArray[i].Contains("MONTHLY"))
                {
                    MONTHLY = ruleArray[i];
                }
                if (ruleArray[i].Contains("YEARLY"))
                {
                    YEARLY = ruleArray[i];
                }
                if (ruleArray[i].Contains("BYSETPOS"))
                {
                    BYSETPOS = ruleArray[i];
                    BYSETPOSCOUNT = ruleArray[i + 1];
                }
                if (ruleArray[i].Contains("BYDAY"))
                {
                    BYDAYPOSITION = i;
                    BYDAY = ruleArray[i];
                    BYDAYVALUE = ruleArray[i + 1];
                }
                if (ruleArray[i].Contains("BYMONTHDAY"))
                {
                    BYMONTHDAYPOSITION = i;
                    BYMONTHDAY = ruleArray[i];
                    BYMONTHDAYCOUNT = ruleArray[i + 1];
                }
                if (ruleArray[i].Contains("BYMONTH"))
                {
                    BYMONTH = ruleArray[i];
                    BYMONTHCOUNT = ruleArray[i + 1];
                }
            }


        }
        #endregion
    }
}
