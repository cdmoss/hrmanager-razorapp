using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Repositories
{
    public interface IAvailabilityRepo
    {
        Task<List<AvailabilityDto>> GetAvailabilitiesByUser(int userId);
        Task<List<AvailabilityDto>> UpdateUserAvailabilities(List<Availability> availabilities, int userId);
    }

    public class MockAvailabilityRepo : IAvailabilityRepo
    {
        public Task<List<AvailabilityDto>> GetAvailabilitiesByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AvailabilityDto>> UpdateUserAvailabilities(List<Availability> availabilities, int userId)
        {
            throw new NotImplementedException();
        }
    }

    public class MySqlAvailabilityRepo : IAvailabilityRepo
    {
        public Task<List<AvailabilityDto>> GetAvailabilitiesByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AvailabilityDto>> UpdateUserAvailabilities(List<Availability> availabilities, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
