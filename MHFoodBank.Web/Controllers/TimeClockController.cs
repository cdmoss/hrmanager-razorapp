using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Common;
using MHFoodBank.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using MHFoodBank.Web.Data;

namespace MHFoodBank.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TimeClockController : ControllerBase
    {
        private readonly IClockedTimeRepo _clockedTimeRepo;
        private readonly UserManager<AppUser> _userManager;

        public TimeClockController(IClockedTimeRepo clockedTimeRepo, UserManager<AppUser> userManager)
        {
            _clockedTimeRepo = clockedTimeRepo;
            _userManager = userManager;
        }

        [HttpPost("punch-clock")]
        [Authorize(Roles = "Admin, Staff, Volunteer")]
        public async Task<IActionResult> PunchClockAsync(ClockedTimeCreateDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            bool userIsAuthenticated = false;
            if (user != null)
            {
                userIsAuthenticated = await _userManager.CheckPasswordAsync(user, dto.Password);
            }
            var clockInResult = new OperationResponse<object>() { Success = false, Message = "Error: Invalid credentials" };

            if (userIsAuthenticated)
            {
                clockInResult = await _clockedTimeRepo.PunchClock(user.Id, dto.Position);
            }

            return Ok(clockInResult);
        }
    }
}
