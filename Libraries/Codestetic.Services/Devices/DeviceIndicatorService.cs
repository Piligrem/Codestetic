using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.GeoZones;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Web.Services.Devices
{
    public partial class DeviceIndicatorService : IDeviceIndicatorService
    {
        #region Constants
        //private const string USERROLES_ALL_KEY = "Specter.userrole.all-{0}";
        //private const string USERROLES_PATTERN_KEY = "Specter.userrole.";
        #endregion Constants

        #region Fields
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> roleRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IDeviceService deviceService;
        private readonly IRepository<DeviceIndicator> _deviceIndicatorRepository;
        private readonly IRepository<ObjectIndicator> objectIndicatorRepository;
        private readonly IRepository<DeviceSetting> deviceSettingRepository;
        //private readonly IRepository<Session> _userSessionRepository;
        //private readonly ICacheManager _cacheManager;
        //private readonly IEventPublisher _eventPublisher;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ILocalizationService localizationService;
        private readonly ISettingService settingService;
        private readonly IGeoZoneService geoZoneService;
        #endregion Fields

        #region Convert function
        private static Func<int, string> SatellitesIcon = (satellites) =>
        {
            if (1 <= satellites && satellites <= 3)
                return "25";
            if (4 <= satellites && satellites <= 6)
                return "50";
            if (7 <= satellites && satellites <= 9)
                return "75";
            if (satellites >= 10)
                return "100";
            return "00";
        };
        private static Func<double, string> GSMIcon = (gsm) =>
        {
            var value = (int)gsm;

            if (1 <= value && value <= 7)
                return "25";
            if (8 <= value && value <= 14)
                return "50";
            if (15 <= value && value <= 22)
                return "75";
            if (value >= 23)
                return "100";
            return "00";
        };
        private static Func<double, int> GSMValue = (gsm) => { return (int)(gsm / 0.31) > 100 ? 100 : (int)(gsm / 0.31); };
        private static Func<double, string> BatteryIcon = (power) =>
        {
            var value = (int)power;
            if (10 <= value && value <= 30)
                return "25";
            if (31 <= value && value <= 60)
                return "50";
            if (61 <= value && value <= 75)
                return "75";
            if (value > 76)
                return "100";
            return "00";
        };
        private static Func<double, int> BatteryValue = (power) => { return (int)power; };
        #endregion Convert function

        #region Constructors
        public DeviceIndicatorService(
            IRepository<Device> deviceRepository,
            IDeviceService deviceService,
            IRepository<DeviceIndicator> deviceIndicatorRepository,
            IRepository<ObjectIndicator> objectIndicatorRepository,
            IRepository<DeviceSetting> deviceSettingRepository,
            //ICacheManager cacheManager,
            //IRepository<Session> userSessionRepository,
            ISettingService settingService,
            //IEventPublisher eventPublisher,
            IDateTimeHelper dateTimeHelper, 
            ICommonServices commonService,
            ILocalizationService localizationService,
            IGeoZoneService geoZoneService,
            IRepository<User> userRepository,
            IRepository<UserRole> roleRepository)
        {
            //this._cacheManager = cacheManager;
            this._deviceIndicatorRepository = deviceIndicatorRepository;
            this.deviceService = deviceService;
            this.objectIndicatorRepository = objectIndicatorRepository;
            this.geoZoneService = geoZoneService;
            this._deviceRepository = deviceRepository;
            this._userRepository = userRepository;
            this.roleRepository = roleRepository;
            //this._userSessionRepository = userSessionRepository;
            this.settingService = settingService;
            this.deviceSettingRepository = deviceSettingRepository;
            this.dateTimeHelper = dateTimeHelper;
            this.localizationService = localizationService;
            //this._eventPublisher = eventPublisher;
        }
        #endregion Constructors

        #region Methods
        public IList<DeviceIndicator> GetAllDeviceIndicatorByUser(User user)
        {
            if (user == null)
                return null;

            var devices = user.Devices;
            var indicators = _deviceIndicatorRepository.Table.Join(devices, x => x.DeviceId, y => y.Id, (x, y) => x);
            var indicatorsGroup = from i in indicators
                                  group i by i.DeviceId into gi
                                  select new { DeviceId = gi.Key, TimestampOnUtc = gi.Max(g => g.TimestampOnUtc) };

            var query = indicators.Join(indicatorsGroup, x => new { x.DeviceId, x.TimestampOnUtc }, y => new { y.DeviceId, y.TimestampOnUtc }, (x, y) => x);
            return query.ToList();
        }
        public DeviceIndicatorIcon GetDeviceIndicator(ObjectIndicator objectIndicator)
        {
            return IconIndicator(objectIndicator, false);
        }
        //public IList<DeviceIndicatorIcon> GetAllDevicesIndicatorByUser(User user)
        //{
        //    var deviceTable = deviceService.GetAllDevicesByUser(user);
        //    var indicatorTable = deviceTable
        //        .GroupBy(dt => new { UserId = dt.Users.FirstOrDefault().Id, DeviceId = dt.Id })
        //        .Select(q => new ObjectIndicator()
        //        {
        //            Id = q.Key.DeviceId,
        //            UserId = q.Key.UserId,
        //            TimestampOnUtc = q.OrderByDescending(gq => gq.DeviceIndicator.TimestampOnUtc).FirstOrDefault().DeviceIndicator.TimestampOnUtc,
        //            SessionDateUtc = q.OrderByDescending(gq => gq.DeviceIndicator.TimestampOnUtc).FirstOrDefault().DeviceIndicator.TimestampOnUtc,
        //            Satellites = q.OrderByDescending(gq => gq.DeviceIndicator.TimestampOnUtc).FirstOrDefault().DeviceIndicator.Satellites,
        //            GSM = q.OrderByDescending(gq => gq.DeviceIndicator.TimestampOnUtc).FirstOrDefault().DeviceIndicator.GSM,
        //            Battery = q.OrderByDescending(gq => gq.DeviceIndicator.TimestampOnUtc).FirstOrDefault().DeviceIndicator.Battery,
        //            ExtendedInfo = q.OrderByDescending(gq => gq.DeviceIndicator.TimestampOnUtc).FirstOrDefault().DeviceIndicator.ExtendedInfo,
        //        });
        //    var indicatorDeviceIcon = new List<DeviceIndicatorIcon>();
        //    indicatorTable.Each(x => indicatorDeviceIcon.Add(GetDeviceIndicator(x)));
        //    return indicatorDeviceIcon;
        //}

        public IList<DeviceIndicator> GetAllDeviceIndicatorByUserId(long userId)
        {
            if (userId == 0)
                return null;

            var user = _userRepository.GetById(userId);
            return GetAllDeviceIndicatorByUser(user);
        }
        public DeviceIndicator GetDeviceIndicatorByDeviceId(long deviceId)
        {
            if (deviceId == 0)
                return null;

            var indicators = _deviceIndicatorRepository.Table.Where(x => x.DeviceId == deviceId);
            indicators = indicators.OrderByDescending(x => x.TimestampOnUtc);
            return indicators.Any() ? indicators.FirstOrDefault() : null;
        }
        #endregion Methods

        #region Private methods
        private DeviceIndicatorIcon IconIndicator(ObjectIndicator objectIndicator, bool controlGeoZone)
        {
            var deviceIndicatorIcon = new DeviceIndicatorIcon()
            {
                Id = objectIndicator.Id,
                Info = new IndicatorIcon()
                {
                    IconValue = "icon-info",
                    TooltipValue = dateTimeHelper.ConvertToUserTime(objectIndicator.TimestampOnUtc, DateTimeKind.Utc).Value.ToString(CultureInfo.CurrentCulture)
                },
                Satellites = new IndicatorIcon()
                {
                    IconValue = SatellitesIcon(objectIndicator.Satellites),
                    TooltipValue = objectIndicator.Satellites.ToString()
                },
                GSM = new IndicatorIcon()
                {
                    IconValue = GSMIcon(objectIndicator.GSM),
                    TooltipValue = GSMValue(objectIndicator.GSM).ToString()
                },
                Battery = new IndicatorIcon()
                {
                    IconValue = BatteryIcon(objectIndicator.Battery),
                    TooltipValue = BatteryValue(objectIndicator.Battery).ToString()
                },
                GeoZoneInfo = new IndicatorIcon()
                {
                    IconValue = "icon-geozone-info",
                    TooltipValue = localizationService.GetResource("Indicator.ControlGeoZone.{0}".FormatWith(controlGeoZone))
                },
            };
            return deviceIndicatorIcon;
        }
        #endregion Private methods
    }
}
