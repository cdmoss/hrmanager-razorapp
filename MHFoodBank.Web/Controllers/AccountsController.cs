//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Castle.DynamicProxy.Contributors;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using MHFoodBank.Api.Repositories;
//using MHFoodBank.Common;
//using MHFoodBank.Common.Dtos;

//namespace MHFoodBank.Api.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class AccountsController : ControllerBase
//    {
//        private readonly IAccountRepo _accountRepo;

//        public AccountsController(IAccountRepo accountRepo)
//        {
//            _accountRepo = accountRepo;
//        }

//        [HttpGet("all")]
//        [Authorize(Roles = "Admin, SuperAdmin")]
//        public async Task<IActionResult> GetAllAccountsAsync()
//        {
//            var response = new OperationResponse<List<AccountAdminListDto>>()
//            {
//                ResponseDto = await _accountRepo.GetAllAccounts(),
//                Message = "Data retrieved successfully",
//                IsSuccess = true,
//            };

//            return Ok(response);
//        }

//        [HttpGet("{id}")]
//        [Authorize(Roles = "Admin, SuperAdmin")]
//        public async Task<IActionResult> GetAccountsByIdAsync(string id)
//        {
//            var response = new OperationResponse<AccountAdminReadEditDto>()
//            {
//                ResponseDto = await _accountRepo.GetAccountByIdForAdmin(new Guid(id)),
//                Message = "Data retrieved successfully",
//                IsSuccess = true,
//            };

//            return Ok(response);
//        }
//    }
//}
