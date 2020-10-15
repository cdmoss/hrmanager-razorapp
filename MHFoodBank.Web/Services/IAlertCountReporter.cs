using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public interface IAlertCountReporter
    {
        Task<int> ReportAlertCount();
    }
}
