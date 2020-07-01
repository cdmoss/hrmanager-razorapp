using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkplaceAdministrator.Common
{
    public class AccountPosition
    {
        public enum AssociationType
        {
            Preferred,
            Assigned,
            PreferredAndAssigned
        }

        public int Id { get; set; }
        public WorkplaceAccount User { get; set; }
        public Position Position { get; set; }
        public AssociationType Association { get; set; }
    }
}
