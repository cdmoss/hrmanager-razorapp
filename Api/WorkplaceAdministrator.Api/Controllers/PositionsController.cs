using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkplaceAdministrator.Api.Repositories;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionRepo _positionRepo;

        public PositionsController(IPositionRepo positionRepo)
        {
            _positionRepo = positionRepo;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin, SuperAdmin, Employee")]
        public async Task<IActionResult> GetAllAccountsAsync()
        {
            var response = new OperationResponse<List<Position>>()
            {
                ResponseDto = await _positionRepo.GetAllPositions(),
                Message = "Data retrieved successfully",
                IsSuccess = true,
            };

            return Ok(response);
        }
    }
}
