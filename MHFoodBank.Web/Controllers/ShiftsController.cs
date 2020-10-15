//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using MHFoodBank.Common;
//using MHFoodBank.Common.Dtos;
//using System.Timers;

//namespace MHFoodBank.Api.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class ShiftsController : ControllerBase
//    {
//        private readonly IShiftRepo _shiftRepo;

//        public ShiftsController(IShiftRepo shiftRepo)
//        {
//            _shiftRepo = shiftRepo;
//        }

//        //[HttpGet("all")]
//        //[Authorize(Roles = "Admin, SuperAdmin, Employee")]
//        //public IActionResult GetAllShiftsAsync()
//        //{
//        //    var shifts = _shiftRepo.GetAllShifts(),

//        //    var response = new List<ShiftReadEditDto>()
//        //    {
//        //        ResponseDto = _shiftRepo.GetAllShifts(),
//        //        Message = "Data retrieved successfully",
//        //        IsSuccess = true,
//        //    };

//        //    return Ok(response);
//        //}

//        //[HttpGet("{id}")]
//        //[Authorize(Roles = "Admin, SuperAdmin, Employee")]
//        //public async Task<IActionResult> GetsShiftsByVolunteerIdAsync(int id)
//        //{
//        //    var shifts = await _shiftRepo.GetShiftsByVolunteer(id);
//        //    shifts = shifts
//        //        .Where(s => s.StartDate.Date == DateTime.Now.Date).ToList();

//        //    ShiftReadEditDto closestShift = shifts.First();
//        //    double shortestTimeDiff = Math.Abs((closestShift.StartTime - DateTime.Now.TimeOfDay).TotalSeconds);

//        //    foreach (var currentShift in shifts)
//        //    {
//        //        var currentTimeDiff = Math.Abs((currentShift.StartTime - DateTime.Now.TimeOfDay).TotalSeconds);
//        //        if (currentTimeDiff < shortestTimeDiff)
//        //        {
//        //            shortestTimeDiff = currentTimeDiff;
//        //            closestShift = currentShift;
//        //        }
//        //    }

//        //    return Ok(closestShift);
//        //}
//    }
//}
