using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web;
using MHFoodBank.Web.Dtos;
using AutoMapper;
using MHFoodBank.Web.Data.Models;

namespace MHFoodBank.Api.Profiles
{
    public class ShiftProfile : Profile
    {
        public ShiftProfile()
        {
            CreateMap<ShiftReadEditDto, Shift>();
            CreateMap<Shift, ShiftReadEditDto>();
            CreateMap<RecurringShiftReadEditDto, RecurringShift>();
            CreateMap<RecurringShift, RecurringShiftReadEditDto>();
        }
    }
}
