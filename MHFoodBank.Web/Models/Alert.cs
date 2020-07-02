using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data.Models
{
    [Serializable]
    public abstract class Alert
    {
        public int Id { get; set; }
        public VolunteerProfile Volunteer { get; set; }
        public DateTime Date { get; set; }
        public bool HasBeenRead { get; set; }

        protected virtual string GetDescription()
        {
            return null;
        }
    }
}
