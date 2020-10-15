using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;

namespace MHFoodBank.Web.Profiles
{
    public class ClockedTimesProfile : Profile
    {
        public ClockedTimesProfile()
        {
            CreateMap<ClockedTime, ClockedTimeReadDto>().ReverseMap();
        }
    }
}
