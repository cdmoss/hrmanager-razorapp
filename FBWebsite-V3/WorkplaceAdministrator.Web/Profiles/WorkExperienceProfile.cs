using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common.Dtos;
using WorkplaceAdministrator.Web.Data.Models;

namespace WorkplaceAdministrator.Web.Profiles
{
    public class WorkExperienceProfile : Profile
    {
        public WorkExperienceProfile()
        {
            CreateMap<WorkExperience, WorkExperienceDto>();
            CreateMap<WorkExperienceDto, WorkExperience>();
        }
    }
}
