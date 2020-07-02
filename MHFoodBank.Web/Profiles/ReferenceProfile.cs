using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Web.Dtos;
using MHFoodBank.Web.Data.Models;

namespace MHFoodBank.Web.Profiles
{
    public class ReferenceProfile : Profile
    {
        public ReferenceProfile()
        {
            CreateMap<Reference, ReferenceDto>();
            CreateMap<ReferenceDto, Reference>();
        }
    }
}
