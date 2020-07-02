using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Data.Models
{
    [Serializable]
    public class ApplicationAlert : Alert
    {
        protected override string GetDescription()
        {
            return $"Recieved a new application from {Volunteer.FirstName} {Volunteer.LastName}";
        }
    }
}
