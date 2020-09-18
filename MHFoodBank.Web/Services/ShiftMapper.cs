using AutoMapper;
using MHFoodBank.Common;
using MHFoodBank.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHFoodBank.Web.Services
{
    public class ShiftMapper
    {
        private readonly IMapper _mapper;
        public ShiftMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public List<ShiftReadEditDto> MapShiftsToDtos(List<Shift> shiftDomainModels)
        {
            List<ShiftReadEditDto> ShiftDtos = new List<ShiftReadEditDto>();
            // Created two lists of shifts to separate them from the main list
            // The reason we're splitting the main list into two is so that the recurring shifts
            // will be mapped properly to the Dto. If we don't split them, recurrence rule and weekdays
            // will be null for all the recurring shifts.
            //List<RecurringShift> recurringShifts = new List<RecurringShift>();
            List<Shift> shifts = new List<Shift>();

            // Maps the regular shifts, and then map the recurring shifts, and then unify the lists into one lis
            ShiftDtos = _mapper.Map<List<ShiftReadEditDto>>(shifts);

            return ShiftDtos;
        }
    }
}
