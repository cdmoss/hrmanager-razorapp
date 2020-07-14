using AutoMapper;
using MHFoodBank.Web.Data.Models;
using MHFoodBank.Web.Dtos;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
