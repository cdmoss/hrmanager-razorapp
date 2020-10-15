using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;

namespace MHFoodBank.Web.Profiles
{
    public class AvailabilityProfile : Profile
    {
        public AvailabilityProfile()
        {
            CreateMap<Availability, AvailabilityDto>();
            CreateMap<AvailabilityDto, Availability>();
        }
    }
}
