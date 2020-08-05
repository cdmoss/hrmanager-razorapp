using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
