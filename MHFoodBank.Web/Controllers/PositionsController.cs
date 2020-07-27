using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MHFoodBank.Api.Repositories;
using MHFoodBank.Common;

namespace MHFoodBank.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionRepo _positionRepo;

        public PositionsController(IPositionRepo positionRepo)
        {
            _positionRepo = positionRepo;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPositionsAsync()
        {
            var positions = await _positionRepo.GetAllPositions();

            return Ok(positions);
        }
    }
}
