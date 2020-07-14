using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web;
using MHFoodBank.Web.Dtos;

namespace MHFoodBank.Api.Repositories
{
    public interface IAlertRepo
    {
        Task<List<AdminAlertListDto>> GetAllAlerts();
        Task<List<VolunteerShiftRequestListDto>> GetAllShiftRequests();
        Task<ShiftRequestReadDto> GetShiftRequestById(int id);
    }
}
