using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;

namespace MHFoodBank.Web.Profiles
{
    public class ReferenceProfile : Profile
    {
        public ReferenceProfile()
        {
            CreateMap<Reference, ReferenceDto>();
            CreateMap<ReferenceDto, Reference>();
        }
    }
}
