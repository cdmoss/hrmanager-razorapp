using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHFoodBank.Common;
using MHFoodBank.Web.Data;

namespace MHFoodBank.Api.Repositories
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
        private readonly FoodBankContext _context;

        public MySqlPositionRepo(FoodBankContext context)
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
            return await _context.Positions.ToListAsync();
        }

        public Task<Position> UpdatePosition(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
