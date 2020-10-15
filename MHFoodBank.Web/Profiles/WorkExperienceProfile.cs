using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;


namespace MHFoodBank.Web.Profiles
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
