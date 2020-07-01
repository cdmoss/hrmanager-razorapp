using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;
using System.Reflection.Metadata.Ecma335;
using AutoMapper;

namespace WorkplaceAdministrator.Api.Services
{
    public interface IWorkplaceAuthService
    {
        Task<AuthResponse> SeedAccounts();
        Task<AuthResponse> RegisterAsync(TestRegisterDto registerDto);
        Task<AuthResponse> LoginAsync(LoginDto loginDto);
    }

    public class WorkplaceAuthService : IWorkplaceAuthService
    {
        private readonly UserManager<WorkplaceAccount> _userManger;
        private readonly SignInManager<WorkplaceAccount> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public WorkplaceAuthService(
            UserManager<WorkplaceAccount> userManager,
            SignInManager<WorkplaceAccount> signInManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManger = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<AuthResponse> SeedAccounts()
        {
            WorkplaceAccount admin = new WorkplaceAccount
            {
                Email = "admin@email.com",
                FirstName = "admin",
                LastName = "admin",
                UserName = "admin",
            };

            WorkplaceAccount staff = new WorkplaceAccount
            {
                Email = "staff@email.com",
                FirstName = "staff",
                LastName = "staff",
                UserName = "staff",
            };

            WorkplaceAccount member = new WorkplaceAccount
            {
                Email = "member@email.com",
                FirstName = "member",
                LastName = "member",
                UserName = "member",
            };

            await _userManger.CreateAsync(admin, "P@$$W0rd");
            await _userManger.CreateAsync(staff, "P@$$W0rd");
            await _userManger.CreateAsync(member, "P@$$W0rd");

            await _userManger.AddToRolesAsync(admin, new List<string>() { "SuperAdmin", "Admin", "Employee" });
            await _userManger.AddToRolesAsync(staff, new List<string>() { "Admin", "Employee" });
            await _userManger.AddToRolesAsync(member, new List<string>() { "Employee" });

            return new AuthResponse
            {
                Message = "Database seeded successfully!",
                IsSuccess = true,
            };
        }

        public async Task<AuthResponse> RegisterAsync(TestRegisterDto registerDto)
        {
            if (registerDto == null)
                return new AuthResponse
                {
                    Message = "Invalid credentials received",
                    IsSuccess = false,
                };

            if (registerDto.Password != registerDto.ConfirmPassword)
                return new AuthResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var newAccount = _mapper.Map<WorkplaceAccount>(registerDto);
            newAccount.UserName = registerDto.Email;

            var result = await _userManger.CreateAsync(newAccount, registerDto.Password);

            await _userManger.AddToRolesAsync(newAccount, new List<string>() { "Member" });

            if (result.Succeeded)
            {

                return new AuthResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new AuthResponse
            {
                Message = "User was not created",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };

        }

        public async Task<AuthResponse> LoginAsync(LoginDto loginDto)
        {
            var newAccount = await _userManger.FindByEmailAsync(loginDto.Email);

            if (newAccount == null)
            {
                return new AuthResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false,
                };
            }

            var result = await _userManger.CheckPasswordAsync(newAccount, loginDto.Password);

            if (!result)
                return new AuthResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };

            var roles = await _userManger.GetRolesAsync(newAccount);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newAccount.Id.ToString()),
                new Claim("FirstName", newAccount.FirstName),
                new Claim("LastName", newAccount.LastName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GenerateToken(claims);

            return new AuthResponse
            {
                Message = new JwtSecurityTokenHandler().WriteToken(token),
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        // configure expiry date here
        public JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecret"]));

            return new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        }
    }
}
