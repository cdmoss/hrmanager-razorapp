using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using AutoMapper;

namespace MHFoodBank.Web.Profiles
{
    public class ShiftProfile : Profile
    {
        public ShiftProfile()
        {
            CreateMap<ShiftReadEditDto, Shift>().ForMember(x => x.Volunteer, opt => opt.Ignore()).ForMember(v => v.Position, opt => opt.Ignore());
            CreateMap<Shift, ShiftReadEditDto>();
        }
    }
}
