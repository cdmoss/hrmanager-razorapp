using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common.Dtos;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Web.Profiles
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
