using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Services.Helpers;
using System.Data.Entity;
using Codestetic.Web.Core.Domain.GPS;

namespace Codestetic.Web.Services.Devices
{
    public class DeviceReportService : IDeviceReportService
    {
        #region Fields
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DevicePosition> _devicePositionRepository;
        private readonly IDevicePositionService _devicePositionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        #endregion Fields

        #region Constructor
        public DeviceReportService(IRepository<Device> deviceRepository,
            IRepository<DevicePosition> devicePositionRepository, IDevicePositionService devicePositionService,
            IDateTimeHelper dateTimeHelper
            )
        {
            this._deviceRepository = deviceRepository;
            this._devicePositionRepository = devicePositionRepository;
            this._devicePositionService = devicePositionService;
            this._dateTimeHelper = dateTimeHelper;
        }
        #endregion Constructor

        public int NowConnectedDevices()
        {
            var query = _devicePositionRepository.Table;
            query = query.Where(d => DbFunctions.DiffHours(d.DateOnUtc, DateTime.UtcNow) <= 1);

            return query.Count();
        }

        public int TodayConnected()
        {
            var query = _devicePositionRepository.Table;
            query = query.Where(d => DbFunctions.TruncateTime(d.DateOnUtc) == DbFunctions.TruncateTime(DateTime.UtcNow));

            return query.Count();
        }

        public int TodayActiveDevices()
        {
            var query = _deviceRepository.Table;
            query = query.Where(d => d.Active);

            return query.Count();
        }

        public int TodayDeactivatedDevices()
        {
            var query = _deviceRepository.Table;
            query = query.Where(d => !d.Active);

            return query.Count();
        }
    }
}
