using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common.Dtos;

namespace WorkplaceAdministrator.Blazor.Repositories
{
    public interface IShiftRepo
    {
        Task<List<ShiftReadEditDto>> GetAllShifts();
        Task<bool> CreateShift(ShiftCreateDto shiftDto);
    }

    public class ShiftRepo : IShiftRepo
    {
        private readonly IHttpService _httpService;

        public ShiftRepo(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<bool> CreateShift(ShiftCreateDto shiftDto)
        {
            return false;
           // await _httpService.Get<ShiftCreateDto>("https://localhost:44335/api/shifts/all");
        }

        public async Task<List<ShiftReadEditDto>> GetAllShifts()
        {
            var response = await _httpService.Get<List<ShiftReadEditDto>>("https://localhost:44335/api/shifts/all");
            if (response.IsSuccess)
            {
                return response.ResponseDto;
            }
            else
            {
                return null;
            }
        }
    }
}
