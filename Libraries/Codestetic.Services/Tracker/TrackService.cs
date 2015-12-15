using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.GeoZones;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Core.Domain.Tracker;
using Codestetic.Web.Services.Helpers;
using System.Globalization;
using Codestetic.Web.Core.Domain.Devices;

namespace Codestetic.Web.Services.Tracker
{
    public partial class TrackService : ITrackService
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IRepository<Track> _trackRepository;
        private readonly IRepository<PolygonPoint> _polygonRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDateTimeHelper dateHelper;
        #endregion Fields

        #region Conctructors
        public TrackService(IWorkContext workContext,
            IDbContext dbContext,
            IRepository<Track> trackRepository,
            IRepository<PolygonPoint> polygonRepository,
            IRepository<Device> deviceRepository,
            ICommonServices commonServices,
            ILogger logger,
            IDateTimeHelper dateHelper,
            IEventPublisher eventPublisher)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._trackRepository = trackRepository;
            this._deviceRepository = deviceRepository;
            this._polygonRepository = polygonRepository;
            this._commonServices = commonServices;
            this._logger = logger;
            this._eventPublisher = eventPublisher;
            this.dateHelper = dateHelper;
            //_commonServices.UserActivity
        }
        #endregion Conctructors

        #region Properties
        #endregion Properties

        #region Methods
        public IList<Track> GetTrackByDevice(Device device, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (device == null)
                return null;

            return GetTrackByDeviceId(device.Id, fromDate, toDate);
        }
        public IList<Track> GetTrackByDeviceId(long deviceId, DateTime? dateStart = null, DateTime? dateEnd = null)
        {

            var query = _trackRepository.Table;
            query = query.Where(d => d.DeviceId == deviceId && d.Valid);

            if (dateStart != null) 
            { 
                var ds = Convert.ToDateTime(dateHelper.ConvertToUtcTime(dateStart.Value, DateTimeKind.Local), CultureInfo.InvariantCulture);
                query = query.Where(q => q.DateOnUtc >= ds);
            }
            if (dateEnd != null)
            {
                var de = dateHelper.ConvertToUtcTime(dateEnd.Value, DateTimeKind.Local);
                query = query.Where(q => q.DateOnUtc <= de);
            }
            query = query.OrderByDescending(d => d.TimestampOnUtc);

            return query.ToList();
        }
        public Track GetTrackById(long id)
        {
            if (id == 0)
                return null;

            var track = _trackRepository.GetById(id);

            return track;
        }
        public DateTime GetMinDateTrack(long deviceId)
        {
            if (deviceId == 0)
                return DateTime.UtcNow;

            var query = _trackRepository.Table;
            query = query.Where(x => x.DeviceId == deviceId);

            if (!query.Any())
                return DateTime.UtcNow;

            var minDate = query.Min(q => q.DateOnUtc);
            return minDate;
        }
        public DateTime GetMaxDateTrack(long deviceId)
        {
            if (deviceId == 0)
                return DateTime.UtcNow;

            var query = _trackRepository.Table;
            query = query.Where(x => x.DeviceId == deviceId);

            if (!query.Any())
                return DateTime.UtcNow;

            var maxDate = query.Max(q => q.DateOnUtc);
            return maxDate;
        }
        #endregion Methods

        #region Helpers
        #endregion Helpers
    }
}
