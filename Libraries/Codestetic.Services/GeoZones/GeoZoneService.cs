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
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Core.Domain.GPS;

namespace Codestetic.Web.Services.GeoZones
{
    public partial class GeoZoneService : IGeoZoneService
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IRepository<GeoZone> _geoZoneRepository;
        private readonly IRepository<PolygonPoint> _polygonRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        #endregion Fields

        #region Conctructors
        public GeoZoneService(IWorkContext workContext,
            IDbContext dbContext,
            IRepository<GeoZone> geoZoneRepository,
            IRepository<PolygonPoint> polygonRepository,
            IRepository<Device> deviceRepository,
            ICommonServices commonServices,
            ILogger logger,
            IEventPublisher eventPublisher)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._geoZoneRepository = geoZoneRepository;
            this._deviceRepository = deviceRepository;
            this._polygonRepository = polygonRepository;
            this._commonServices = commonServices;
            this._logger = logger;
            this._eventPublisher = eventPublisher;
            //_commonServices.UserActivity
        }
        #endregion Conctructors

        #region Properties
        #endregion Properties

        #region Methods
        public IList<GeoZone> GetGeoZonesByDevice(Device device)
        {
            if (device == null)
                return null;

            return GetGeoZonesByDeviceId(device.Id);
        }
        public IList<GeoZone> GetGeoZonesByDeviceId(long deviceId)
        {
            var geoZones = _deviceRepository.GetById(deviceId).GeoZones;
            return geoZones.ToList();
        }
        public GeoZone GetGeoZoneById(long id)
        {
            if (id == 0)
                return null;

            var geoZone = _geoZoneRepository.GetById(id);

            return geoZone;
        }
        public virtual IList<GeoZone> GetGeoZonesByUser(User user)
        {
            var g = user.GeoZones.FirstOrDefault();
            return user.GeoZones.ToList();
        }
        public virtual GeoZone GetGeoZoneByName(string name)
        {
            if (!name.HasValue())
                return null;

            var query = _geoZoneRepository.Table;
            query = query.Where(z => z.Name == name);

            return query.FirstOrDefault();
        }

        public virtual void SaveGeoZone(GeoZone geoZone)
        {
            if (geoZone == null)
                throw new ArgumentNullException("geoZone");

            var gZ = _geoZoneRepository.GetById(geoZone.Id);

            if (gZ == null)
                InsertGeoZone(geoZone);
            else
                UpdateGeoZone(geoZone);
        }

        #region GeoZone
        public virtual void InsertGeoZone(GeoZone geoZone)
        {
            if (geoZone == null)
                throw new ArgumentNullException("geoZone");

            _geoZoneRepository.Insert(geoZone);

            //event notification
            _eventPublisher.EntityInserted(geoZone);
        }
        public virtual void UpdateGeoZone(GeoZone geoZone)
        {
            if (geoZone == null)
                throw new ArgumentNullException("geoZone");

            geoZone.UpdateOnUtc = DateTime.UtcNow;
            _geoZoneRepository.Update(geoZone);

            //event notification
            _eventPublisher.EntityUpdated(geoZone);
        }
        public virtual void DeleteGeoZone(GeoZone geoZone)
        {
            if (geoZone == null)
                throw new ArgumentNullException("geoZone");

            _geoZoneRepository.Delete(geoZone);

            //event notification
            _eventPublisher.EntityDeleted(geoZone);
        }
        public virtual void DeleteGeoZoneByIds(IList<long> ids)
        {
            if (ids.Count() == 0)
                return;

            ids.Each(z => {
                var zone = _geoZoneRepository.GetById(z);
                DeleteGeoZone(zone);
            });
        }
        #endregion GeoZone

        #endregion Methods

        #region Helpers
        #endregion Helpers
    }
}
