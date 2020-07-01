using Blazored.LocalStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Blazor.Repositories
{
    public interface IAccountRepo
    {
        Task<List<AccountAdminListDto>> GetAllUsers();
        Task<List<AccountAdminListDto>> SearchUsers(string name, int positionId);
        Task<AccountAdminReadEditDto> GetUserByIdForAdmin(Guid id);
        Task<AccountReadEditDto> GetUserByIdForUser(Guid id);
        Task<AccountAdminReadEditDto> UpdateUserByAdmin(AccountAdminReadEditDto user);
        Task<AccountReadEditDto> UpdateUserByUser(AccountReadEditDto user);
        Task<AccountAdminReadEditDto> DeleteUser(Guid id);
    }
    public class AccountRepo : IAccountRepo
    {
        private readonly IHttpService _httpService;

        public AccountRepo(IHttpService httpService)
        {
            _httpService = httpService; 
        }

        public Task<AccountAdminReadEditDto> DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AccountAdminListDto>> GetAllUsers()
        {
            var response = await _httpService.Get<List<AccountAdminListDto>>("https://localhost:44335/api/accounts/all");

            if (response.IsSuccess)
            {
                var accountList = response.ResponseDto;
                return accountList;
            }
            else
            {
                return null;
            }
        }

        public async Task<AccountAdminReadEditDto> GetUserByIdForAdmin(Guid id)
        {
            var response = await _httpService.Get<AccountAdminReadEditDto>($"https://localhost:44335/api/accounts/{id}");

            if (response.IsSuccess)
            {
                var account = response.ResponseDto;
                return account;
            }
            else
            {
                return null;
            }
        }

        public Task<AccountReadEditDto> GetUserByIdForUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountAdminListDto>> SearchUsers(string name, int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<AccountAdminReadEditDto> UpdateUserByAdmin(AccountAdminReadEditDto user)
        {
            throw new NotImplementedException();
        }

        public Task<AccountReadEditDto> UpdateUserByUser(AccountReadEditDto user)
        {
            throw new NotImplementedException();
        }
    }
}
