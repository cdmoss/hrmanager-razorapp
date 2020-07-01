using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Repositories
{
    public interface IAlertRepo
    {
        Task<List<AdminAlertListDto>> GetAllAlerts();
        Task<List<UserShiftRequestListDto>> GetAllShiftRequests();
        Task<ShiftRequestReadDto> GetShiftRequestById(int id);
    }
}
