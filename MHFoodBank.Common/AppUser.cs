using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Common;
using Microsoft.AspNetCore.Identity;

namespace MHFoodBank.Common
{
    [Serializable]
    public class AppUser : IdentityUser<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        public VolunteerProfile VolunteerProfile { get; set; }
    }
}
