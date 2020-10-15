using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MHFoodBank.Web.Areas.Admin.Pages.Shared
{
    public class AdminPageModel : PageModel
    {
        public int NotifcationCount { get; set; }
        public string FullName { get; set; }
        public string CurrentPage { get; set; }

        public AdminPageModel(string currentPage)
        {
            CurrentPage = currentPage;
        }
    }
}
