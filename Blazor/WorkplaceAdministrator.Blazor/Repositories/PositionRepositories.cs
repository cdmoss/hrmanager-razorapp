using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Blazor.Repositories
{
    public interface IPositionRepo
    {
        Task<List<Position>> GetAllPositions();
        Task<bool> UpdatePosition(Position position);
        Task<bool> DeletePosition(Position position);
    }

    public class PositionRepo : IPositionRepo
    {
        private readonly IHttpService _httpService;

        public PositionRepo(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public Task<bool> DeletePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Position>> GetAllPositions()
        {
            var response = await _httpService.Get<List<Position>>("https://localhost:44335/api/shifts/all");

            if (response.IsSuccess)
            {
                return response.ResponseDto;
            }
            else
            {
                return null;
            }
        }

        public Task<bool> UpdatePosition(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
