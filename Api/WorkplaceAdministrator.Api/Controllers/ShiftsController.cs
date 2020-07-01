using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkplaceAdministrator.Api.Repositories;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftRepo _shiftRepo;

        public ShiftsController(IShiftRepo shiftRepo)
        {
            _shiftRepo = shiftRepo;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin, SuperAdmin, Employee")]
        public async Task<IActionResult> GetAllAccountsAsync()
        {
            var response = new OperationResponse<List<ShiftReadEditDto>>()
            {
                ResponseDto = _shiftRepo.GetAllShifts(),
                Message = "Data retrieved successfully",
                IsSuccess = true,
            };

            return Ok(response);
        }
    }
}
