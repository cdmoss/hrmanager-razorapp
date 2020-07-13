using AutoMapper;
using MHFoodBank.Web.Data.Models;
using MHFoodBank.Web.Dtos;
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
            // Created two lists of shifts to serparate them from the main list
            // The reason we're splitting the main list into two is so that the recurring shifts
            // will be mapped properly to the Dto. If we don't split them, recurrence rule and weekdays
            // will be null for all the recurring shifts.
            List<RecurringShift> recurringShifts = new List<RecurringShift>();
            List<Shift> shifts = new List<Shift>();

            // Foreach loop created to split the shifts to the shifts list and the recurring shifts list
            foreach (Shift x in shiftDomainModels)
            {
                if (x is RecurringShift recurringShift)
                {
                    recurringShifts.Add(recurringShift);
                }
                else
                {
                    shifts.Add(x);
                }
            }

            // Maps the regular shifts, and then map the recurring shifts, and then unify the lists into one lis
            ShiftDtos = _mapper.Map<List<ShiftReadEditDto>>(shifts);
            ShiftDtos = ShiftDtos.Concat(_mapper.Map<List<ShiftReadEditDto>>(recurringShifts)).ToList();

            return ShiftDtos;
        }
    }
}
