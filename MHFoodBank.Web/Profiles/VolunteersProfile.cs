using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;

namespace MHFoodBank.Web.Profiles
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
