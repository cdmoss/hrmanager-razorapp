using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;

namespace MHFoodBank.Web.Profiles
{
    public class AlertsProfile : Profile
    {
        public AlertsProfile()
        {
            CreateMap<ShiftRequestReadDto, ShiftRequestAlert>().ReverseMap();
            CreateMap<ShiftRequestAlert, AdminAlertListDto>().ReverseMap();
            CreateMap<ApplicationAlert, AdminAlertListDto>().ReverseMap();
        }
    }
}
