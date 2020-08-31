using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MailKit.Net.Imap;
using AutoMapper;

namespace MHFoodBank.Web.Areas.Admin.Pages.Shared
{
    public class AdminPageModel : PageModel
    {
        protected readonly FoodBankContext _context;

        public int NotifcationCount { get; set; }
        public string FullName { get; set; }
        public string CurrentPage { get; set; }

        public AdminPageModel(FoodBankContext context, string currentPage)
        {
            _context = context;
            NotifcationCount = context.Alerts.Count(a => !a.Read);
            CurrentPage = currentPage;
        }
    }
}
