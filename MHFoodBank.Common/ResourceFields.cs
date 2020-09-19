using System;
using System.Collections.Generic;
using System.Text;

namespace MHFoodBank.Common
{
    public class ResourceFields
    {
        public string Text { set; get; }
        public string Id { set; get; }
        public string GroupId { set; get; }
        public string Color { set; get; }
        public int WorkHourStart { set; get; }
        public int WorkHourEnd { set; get; }
        public List<string> CustomDays { set; get; }
    }
}
