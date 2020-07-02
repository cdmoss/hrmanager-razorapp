using System;
using System.Collections.Generic;
using System.Text;
using MHFoodBank.Web.Data.Models;

namespace MHFoodBank.Web.Dtos
{
    public class ClockedTimeDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Position Position { get; set; }
        public VolunteerMinimalDto Volunteer { get; set; }
    }
}
