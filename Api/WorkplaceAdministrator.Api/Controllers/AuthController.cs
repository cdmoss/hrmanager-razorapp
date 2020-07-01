using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkplaceAdministrator.Api.Services;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Controllers
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IWorkplaceAuthService _authService;

        public AuthController(IWorkplaceAuthService authService)
        {
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost("seed")]
        public async Task<IActionResult> SeedAsync()
        {
            AuthResponse response = await _authService.SeedAccounts();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            AuthResponse response = await _authService.LoginAsync(loginDto);

            return Ok(response);

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(TestRegisterDto registerDto)
        {
            AuthResponse response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }
    }
}
