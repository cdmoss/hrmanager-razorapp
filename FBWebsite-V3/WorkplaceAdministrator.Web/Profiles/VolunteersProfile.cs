using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Api.Profiles
{
    public class VolunteersProfile : Profile
    {
        public VolunteersProfile()
        {
            // source --> target
            CreateMap<VolunteerProfile, VolunteerReadEditDto>();
            CreateMap<VolunteerProfile, VolunteerMinimalDto>();
            CreateMap<VolunteerProfile, VolunteerAdminReadEditDto>();
            CreateMap<VolunteerAdminReadEditDto, VolunteerProfile>();
        }
    }
}
