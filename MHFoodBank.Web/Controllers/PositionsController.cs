using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MHFoodBank.Common;
using MHFoodBank.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace MHFoodBank.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly FoodBankContext _context;

        public PositionsController(FoodBankContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPositionsAsync()
        {
            var positions = await _context.Positions.Where(p => p.Name != "All").ToListAsync();

            return Ok(positions);
        }
    }
}
