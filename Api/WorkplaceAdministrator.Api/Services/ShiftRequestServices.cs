using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkplaceAdministrator.Api.Repositories;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Api.Services
{
    public interface IShiftRequestService
    {
        Task<bool> AcceptShiftRequest(int shiftRequestId);
        Task<bool> DeclineShiftRequest(int shiftRequestId);
    }

    public class MockShiftRequestService : IShiftRequestService
    {
        private readonly IAlertRepo _requestRepo;

        public MockShiftRequestService(IAlertRepo requestRepo)
        {
            _requestRepo = requestRepo;
        }

        public Task<bool> AcceptShiftRequest(int shiftRequestId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeclineShiftRequest(int shiftRequestId)
        {
            throw new NotImplementedException();
        }
    }

    public class MySqlShiftRequestService : IShiftRequestService
    {
        private readonly IAlertRepo _requestRepo;

        public MySqlShiftRequestService(IAlertRepo requestRepo)
        {
            _requestRepo = requestRepo;
        }

        public Task<bool> AcceptShiftRequest(int shiftRequestId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeclineShiftRequest(int shiftRequestId)
        {
            throw new NotImplementedException();
        }
    }
}
