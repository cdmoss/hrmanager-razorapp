using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;
using AutoMapper;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Api.Profiles
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
