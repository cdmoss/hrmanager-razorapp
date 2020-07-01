using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Api.Data;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Api.Repositories
{
    public interface IPositionRepo
    {
        Task<List<Position>> GetAllPositions();
        Task<Position> CreatePosition(Position position);
        Task<Position> UpdatePosition(Position position);
        Task<Position> DeletePosition(Position position);
    }

    public class MockPositionRepo : IPositionRepo
    {
        public Task<Position> CreatePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public Task<Position> DeletePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public Task<List<Position>> GetAllPositions()
        {
            throw new NotImplementedException();
        }

        public Task<Position> UpdatePosition(Position position)
        {
            throw new NotImplementedException();
        }
    }

    public class MySqlPositionRepo : IPositionRepo
    {
        private readonly WorkplaceDbContext _context;

        public MySqlPositionRepo(WorkplaceDbContext context)
        {
            _context = context;
        }

        public Task<Position> CreatePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public Task<Position> DeletePosition(Position position)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Position>> GetAllPositions()
        {
            return await _context.Position.ToListAsync();
        }

        public Task<Position> UpdatePosition(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
