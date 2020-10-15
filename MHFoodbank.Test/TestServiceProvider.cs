using AutoMapper;
using MHFoodBank.Web.Profiles;
using Microsoft.Extensions.Configuration;

namespace MHFoodbank.Test
{
    public class TestServiceProvider
    {
        public IConfiguration GetConfig()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public IMapper GetMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new VolunteersProfile());
                cfg.AddProfile(new AlertsProfile());
                cfg.AddProfile(new AvailabilityProfile());
                cfg.AddProfile(new ClockedTimesProfile());
                cfg.AddProfile(new ReferenceProfile());
                cfg.AddProfile(new ShiftProfile());
                cfg.AddProfile(new WorkExperienceProfile());
            });

            return mapperConfig.CreateMapper();
        }
    }
}
