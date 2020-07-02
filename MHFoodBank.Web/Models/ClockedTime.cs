using System;
using System.Collections.Generic;
using System.Text;
using MHFoodBank.Web.Data.Models;

namespace MHFoodBank.Web.Models
{
    public class ClockedTime
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Position Position { get; set; }
        public VolunteerProfile VolunteerProfile { get; set; }
    }
}
