using MHFoodBank.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            CurrentPage = currentPage;
            _context = context;
        }
    }
}
