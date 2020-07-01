using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkplaceAdministrator.Api.Data;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Repositories
{
    public interface IAccountRepo
    {

        Task<List<AccountAdminListDto>> GetAllAccounts();
        Task<List<AccountAdminListDto>> SearchAccounts(string name, int positionId);
        Task<AccountAdminReadEditDto> GetAccountByIdForAdmin(Guid id);
        Task<AccountReadEditDto> GetAccountByIdForAccount(Guid id);
        Task<AccountAdminReadEditDto> UpdateAccountByAdmin(AccountAdminReadEditDto user);
        Task<AccountReadEditDto> UpdateAccountByUser(AccountReadEditDto user);
        Task<AccountAdminReadEditDto> DeleteAccounts(Guid id);
    }

    public class MockRepo : IAccountRepo
    {
        public Task<AccountAdminReadEditDto> DeleteAccounts(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountAdminListDto>> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public Task<AccountAdminReadEditDto> GetAccountByIdForAdmin(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AccountReadEditDto> GetAccountByIdForAccount(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountAdminListDto>> SearchAccounts(string name, int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<AccountAdminReadEditDto> UpdateAccountByAdmin(AccountAdminReadEditDto user)
        {
            throw new NotImplementedException();
        }

        public Task<AccountReadEditDto> UpdateAccountByUser(AccountReadEditDto user)
        {
            throw new NotImplementedException();
        }
    }

    public class MySqlAccountRepo : IAccountRepo
    {
        private readonly WorkplaceDbContext _context;
        private readonly IMapper _mapper;

        public MySqlAccountRepo(WorkplaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<AccountAdminReadEditDto> DeleteAccounts(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AccountAdminListDto>> GetAllAccounts()
        {
            // TODO: filter out admins and staff

            var employees = await _context.Accounts.ToListAsync();
            var dtos = _mapper.Map<List<AccountAdminListDto>>(employees);

            return dtos;
        }

        public async Task<AccountAdminReadEditDto> GetAccountByIdForAdmin(Guid id)
        {
            var employees = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            var dto = _mapper.Map<AccountAdminReadEditDto>(employees);

            return dto;
        }

        public Task<AccountReadEditDto> GetAccountByIdForAccount(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountAdminListDto>> SearchAccounts(string name, int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<AccountAdminReadEditDto> UpdateAccountByAdmin(AccountAdminReadEditDto user)
        {
            throw new NotImplementedException();
        }

        public Task<AccountReadEditDto> UpdateAccountByUser(AccountReadEditDto user)
        {
            throw new NotImplementedException();
        }
    }
}