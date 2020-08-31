using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web;
using MHFoodBank.Common.Dtos;
using MHFoodBank.Common;

namespace MHFoodBank.Api.Profiles
{
    public class VolunteersProfile : Profile
    {
        public VolunteersProfile()
        {
            // source --> target
            CreateMap<VolunteerProfile, VolunteerReadEditDto>();
            CreateMap<VolunteerProfile, VolunteerMinimalDto>();
            CreateMap<VolunteerMinimalDto, VolunteerProfile>();
            CreateMap<VolunteerProfile, VolunteerAdminReadEditDto>();
            CreateMap<VolunteerAdminReadEditDto, VolunteerProfile>();
            CreateMap<RegisterDto, VolunteerProfile>();
            CreateMap<StaffRegisterDto, VolunteerProfile>();
        }
    }
}
