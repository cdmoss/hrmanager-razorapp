using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Api.Data;
using WorkplaceAdministrator.Common;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Api.Repositories
{
    public interface IShiftRepo
    {
        List<ShiftReadEditDto> GetAllShifts();
        Task<List<ShiftReadEditDto>> GetShiftsByVolunteer(int volunteerId);
        Task<List<ShiftReadEditDto>> GetShiftsByPosition(int positionId);
        Task<ShiftReadEditDto> GetShiftById(int id);
        Task<ShiftReadEditDto> CreateShift(ShiftCreateDto shift);
        Task<ShiftReadEditDto> UpdateShift(ShiftReadEditDto shift);
        Task<ShiftReadEditDto> DeleteShift(int id);
    }

    public class MockShiftRepo : IShiftRepo
    {
        public Task<ShiftReadEditDto> CreateShift(ShiftCreateDto shift)
        {
            throw new NotImplementedException();
        }

        public Task<ShiftReadEditDto> DeleteShift(int id)
        {
            throw new NotImplementedException();
        }

        public List<ShiftReadEditDto> GetAllShifts()
        {
            throw new NotImplementedException();
        }

        public Task<ShiftReadEditDto> GetShiftById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShiftReadEditDto>> GetShiftsByPosition(int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShiftReadEditDto>> GetShiftsByVolunteer(int volunteerId)
        {
            throw new NotImplementedException();
        }

        public Task<ShiftReadEditDto> UpdateShift(ShiftReadEditDto shift)
        {
            throw new NotImplementedException();
        }
    }

    public class MySqlShiftRepo : IShiftRepo
    {
        private readonly WorkplaceDbContext _context;
        private readonly IMapper _mapper;

        public MySqlShiftRepo(WorkplaceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<ShiftReadEditDto> CreateShift(ShiftCreateDto shift)
        {
            throw new NotImplementedException();
        }

        public Task<ShiftReadEditDto> DeleteShift(int id)
        {
            throw new NotImplementedException();
        }

        public List<ShiftReadEditDto> GetAllShifts()
        {
            var shifts = _context.Shifts.ToList();
            var dtos = _mapper.Map<List<ShiftReadEditDto>>(shifts);

            return dtos;
        }

        public Task<ShiftReadEditDto> GetShiftById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShiftReadEditDto>> GetShiftsByPosition(int positionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShiftReadEditDto>> GetShiftsByVolunteer(int volunteerId)
        {
            throw new NotImplementedException();
        }

        public Task<ShiftReadEditDto> UpdateShift(ShiftReadEditDto shift)
        {
            throw new NotImplementedException();
        }
    }
}
