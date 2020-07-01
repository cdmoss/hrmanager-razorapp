using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            // source --> target
            CreateMap<RegisterDto, WorkplaceAccount>();
            CreateMap<TestRegisterDto, WorkplaceAccount>();
            CreateMap<WorkplaceAccount, AccountAdminListDto>();
            CreateMap<WorkplaceAccount, AccountAdminReadEditDto>();
        }
    }
}
