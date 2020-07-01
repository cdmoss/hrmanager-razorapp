using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MailKit.Net.Imap;
using AutoMapper;

namespace WorkplaceAdministrator.Web.Areas.Admin.Pages.Shared
{
    public class AdminPageModel : PageModel
    {
        protected readonly FoodBankContext _context;

        public int NotifcationCount { get; set; }
        public string FullName { get; set; }

        public AdminPageModel(FoodBankContext context)
        {
            _context = context;
            NotifcationCount = context.Alerts.Count(a => a.HasBeenRead == false);
        }
    }
}
