using MHFoodBank.Web.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Models
{
    // this class links a shift that has been edited from a recurring set 
    // to it's parent recurring set and preserves its original properties
    public class RecurringChildLink
    {
        public int Id { get; set; }
        public RecurringShift ParentSet { get; set; }
        public Shift OriginalShift { get; set; }
        public Shift NewShift { get; set; }
    }
}
