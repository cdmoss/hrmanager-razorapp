using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web;
using MHFoodBank.Common.Dtos;
using AutoMapper;
using MHFoodBank.Common;

namespace MHFoodBank.Api.Profiles
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
