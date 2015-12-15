using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Core.Domain.Common;
using System.Diagnostics;
using Codestetic.Web.Core.Domain.GeoZones;
using Codestetic.Web.Services.GeoZones;
using Codestetic.Web.Core.Domain.Tracker;
using System.Data.Entity.Spatial;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Domain.Devices;

namespace Codestetic.Web.Services.Devices
{
    public partial class DeviceService : IDeviceService
    {
        #region Constants
        private const string DEVICES_ALL_KEY = "specter.device.all-{0}";
        private const string DEVICES_COUNT_BY_USERID_KEY = "specter.device.user.id.count-{0}";
        private const string DEVICES_PATTERN_KEY = "specter.device.";
        private const string DEVICESTYPE_PATTERN_KEY = "specter.devicetype.";
        private const string DEVICESETTING_PATTERN_KEY = "specter.devicesetting.";
        private const string DEVICES_BY_USERID_KEY = "specter.device.user.id-{0}";
        private const string DEVICES_BY_ID_KEY = "specter.device.id-{0}";
        #endregion Constants

        #region Fields
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<DeviceType> _deviceTypeRepository;
        private readonly IRepository<DeviceSetting> _deviceSettingRepository;
        private readonly IRepository<DevicePosition> _devicePositionRepository;
        private readonly IRepository<DeviceEvent> _deviceEventRepository;
        private readonly IRepository<DeviceIndicator> _deviceIndicatorRepository;
        private readonly IRepository<Track> _trackRepository;
        private readonly IGeoZoneService _geoZoneService;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;
        private readonly DeviceDefaultSettings _deviceDefaultSettings;
        #endregion Fields

        #region Constructors
        public DeviceService(IRepository<User> userRepository,
            IRepository<Device> deviceRepository,
            IRepository<DeviceType> deviceTypeRepository,
            IRepository<DeviceSetting> deviceSettingRepository,
            IRepository<DevicePosition> devicePositionRepository,
            IRepository<DeviceEvent> deviceEventRepository,
            IRepository<DeviceIndicator> deviceIndicatorRepository,
            IRepository<Track> trackRepository, IGeoZoneService geoZoneService,
            IEventPublisher eventPublisher, ICacheManager cacheManager,
            IUserActivityService userActivityService, DeviceDefaultSettings deviceDefaultSettings,
            CommonSettings commonSettings, ISettingService settingService,
            IDateTimeHelper dateTimeHelper, DateTimeSettings dateTimeSettings)
        {
            this._userRepository = userRepository;
            this._deviceRepository = deviceRepository;
            this._deviceTypeRepository = deviceTypeRepository;
            this._deviceSettingRepository = deviceSettingRepository;
            this._devicePositionRepository = devicePositionRepository;
            this._deviceEventRepository = deviceEventRepository;
            this._deviceIndicatorRepository = deviceIndicatorRepository;
            this._trackRepository = trackRepository;
            this._geoZoneService = geoZoneService;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
            this._userActivityService = userActivityService;
            this._eventPublisher = eventPublisher;
            this._commonSettings = commonSettings;
            this._settingService = settingService;
            this._deviceDefaultSettings = deviceDefaultSettings;
        }
        #endregion Constructors

        #region Methods
        public virtual IPagedList<Device> GetAllDevices(int? pageIndex = null, int? pageSize = null, bool hidden = true)
        {
            var query = _deviceRepository.Table;
            if (hidden)
                query = query.Where(q => q.Active);
            query = query.OrderByDescending(c => c.CreatedOnUtc);
            if (pageIndex == null)
                pageIndex = 0;
            if (pageSize == null)
                pageSize = query.Count();

            var devices = new PagedList<Device>(query, pageIndex.Value, pageSize.Value);
            return devices;
        }

        public IEnumerable<Device> GetAllDevicesByUser(User user)
        {
            if (user == null)
                return null;

            string key = string.Format(DEVICES_BY_USERID_KEY, user.Id);
            return _cacheManager.Get(key, () =>
            {
                //Debug.WriteLine("{0} - Not cached devices user: {1}", "GetAllDevicesByUser", user.Username);
                var devices = user.Devices.Where(device => device.Active);
                devices = devices.OrderByDescending(device => device.Id);
                
                return devices.ToList();
            }, _commonSettings.DeviceUserCacheTime);
        }
        public virtual int GetDevicesCount(User user)
        {
            if (user == null)
                return 0;

            string key = string.Format(DEVICES_COUNT_BY_USERID_KEY, user.Id);
            return _cacheManager.Get(key, () =>
            {
                return user.Devices.Count();

                //var query = deviceRepository.Table;
                //query = query.Where(d => d.UserIds.Contains(userId));
                //query = query.Where(d => d.IsActive);
                //return query.Select(x => x.Id).Count();
            }, _commonSettings.DeviceUserCacheTime);
        }
        public virtual Device GetDeviceById(long id)
        {
            if (id == 0)
                return null;
            var device = _deviceRepository.GetById(id);
            return device;
        }
        public virtual IEnumerable<DeviceSetting> GetAllDevicesSettingByUser(User user)
        {
            if (user == null)
                return null;

            var query = user.Devices.Where(device => device.Active);
            //var settings = query.Select(device => device.DeviceSettings.DefaultIfEmpty(new DeviceSetting() { DeviceId = device.Id }).FirstOrDefault()).ToList();
            var settings = query.Select(device => device.DeviceSetting).ToList();
            return settings;
        }
        public virtual DeviceSetting GetDeviceSettingByDeviceId(long deviceId)
        {
            if (deviceId == 0)
                return null;

            var device = _deviceRepository.GetById(deviceId);
            return device.DeviceSetting;
        }
        public virtual DeviceSetting GetDeviceSettingById(long id)
        {
            if (id == 0)
                return null;

            var setting = _deviceSettingRepository.GetById(id);
            return setting;
        }
        public virtual IList<Device> GetDevicesByGeoZoneId(long id)
        {
            if (id == 0)
                return null;

            var query = _deviceRepository.Table;
            query = query.Where(d => d.GeoZones.Select(z => z.Id).ToList().Contains(id));
            return query.ToList();
        }
        public virtual Device GetDeviceByIMEI(string imei)
        {
            if (imei.IsNullOrEmpty())
                return null;

            var query = _deviceRepository.Table;
            query = query.Where(d => d.IMEI == imei);
            return query.FirstOrDefault();
        }
        public virtual void AddDevice(User user, string name, string imei, long deviceTypeId)
        {
            var device = new Device()
            {
                IMEI = imei,
                Name = name,
                Active = true,
                DeviceTypeId = deviceTypeId,
                CreatedOnUtc = DateTime.UtcNow,
            };
            device.Users.Add(user);
            this.InsertDevice(device);
            device = this.GetDeviceByIMEI(imei);

            var position = new DevicePosition()
            {
                Id = device.Id, // DeviceId
                TimestampOnUtc = DateTime.UtcNow,
                DateOnUtc = DateTime.UtcNow,
                Valid = true,
                Position = Geo.ToGeoPoint(_deviceDefaultSettings.Latitude, _deviceDefaultSettings.Longitude),
                Altitude = 0,
                Angle = 0,
                Speed = 0
            };
            var settings = new DeviceSetting()
            {
                Id = device.Id,
                CreatedOnUtc = DateTime.UtcNow,
                PictureId = 0,
                MarkerId = 0,

                ToleranceEnable = _deviceDefaultSettings.ToleranceEnable,
                ToleranceRadius = _deviceDefaultSettings.ToleranceRadius,
                ToleranceStrokeColor = _deviceDefaultSettings.ToleranceStrokeColor,
                ToleranceStrokeWidth = _deviceDefaultSettings.ToleranceStrokeWidth,
                ToleranceFillColor = _deviceDefaultSettings.ToleranceFillColor,

                TrackEnable = _deviceDefaultSettings.TrackEnable,
                TrackStrokeColor = _deviceDefaultSettings.TrackStrokeColor,
                TrackStrokeWidth = _deviceDefaultSettings.TrackStrokeWidth,

                IntervalUpdateDevice = _deviceDefaultSettings.IntervalUpdateDevice,

                ControlSatellites = _deviceDefaultSettings.ControlSatellites,
                ControlBattery = _deviceDefaultSettings.ControlBattery,
                MinBatteryLevel = _deviceDefaultSettings.MinBatteryLevel,
                ControlGSM = _deviceDefaultSettings.ControlGSM,
                ControlButtonSos = _deviceDefaultSettings.ControlButtonSos,
                ControlInGeoZone = _deviceDefaultSettings.ControlInGeoZone,
                ControlOutGeoZone = _deviceDefaultSettings.ControlOutGeoZone,
            };
            this.InsertDevicePosition(position);
            this.InsertDeviceSetting(settings);
        }

        #region GeoZones
        public virtual void AddGeoZones(Device device, IList<string> geoZonesNames)
        {
            if (device == null)
                throw new ArgumentNullException("device");
            
            if (geoZonesNames.Count() == 0)
                return;

            geoZonesNames.Each(z => {
                var zone = _geoZoneService.GetGeoZoneByName(z);
                device.GeoZones.Add(zone);
            });
        }
        public virtual void RemoveGeoZones(Device device, IList<long> geoZoneIds)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            if (geoZoneIds.Count() == 0)
                return;

            geoZoneIds.Each(g => {
                var geoZone = _geoZoneService.GetGeoZoneById(g);
                device.GeoZones.Remove(geoZone);
            });
        }
        #endregion GeoZones

        #region Location
        public bool SaveLocation(string imei, DateTime dateOnUtc, double latitude, double longitude, double altitude, double angle, double speed, bool alarm)
        {
            var device = GetDeviceByIMEI(imei);
            if (device == null) return false;

            var deviceSettings = _deviceSettingRepository.Where(ds => ds.Id == device.Id).FirstOrDefault();
            var devicePosition = device.DevicePosition;
            var correctTime = devicePosition.DateOnUtc.AddSeconds(deviceSettings.IntervalUpdateDevice);
            var timestamp = DateTime.UtcNow;

            Track track = null;

            if (DateTime.UtcNow <= correctTime) return true;

            var position = Geo.ToGeoPoint(latitude, longitude);

            if (devicePosition.Position == position)
            {
                track = _trackRepository.Table.Where(d => d.DeviceId == device.Id).OrderByDescending(d => d.TimestampOnUtc).FirstOrDefault();

                track.TimestampOnUtc = timestamp;
                track.DateOnUtc = dateOnUtc;
                _trackRepository.Update(track);

                devicePosition.TimestampOnUtc = timestamp;
                devicePosition.DateOnUtc = track.DateOnUtc;
                _devicePositionRepository.Update(devicePosition);
            }
            else
            {
                track = new Track()
                {
                    TimestampOnUtc = timestamp,
                    DeviceId = device.Id,
                    DateOnUtc = dateOnUtc,
                    Valid = true,
                    Position = position,
                    Altitude = altitude,
                    Angle = angle,
                    Speed = speed,
                };

                devicePosition.TimestampOnUtc = timestamp;
                devicePosition.DateOnUtc = dateOnUtc;
                devicePosition.Valid = true;
                devicePosition.Position = position;
                devicePosition.Altitude = altitude;
                devicePosition.Angle = angle;
                devicePosition.Speed = speed;

                _trackRepository.Insert(track);
                _devicePositionRepository.Update(devicePosition);

                Debug.WriteLine("#{0} - Position: Date: {1}, Lat: {2}, Lon: {3}", DateTime.Now, devicePosition.DateOnUtc, latitude, longitude);
            }

            #region Device events
            if (deviceSettings.MinBatteryLevel > 0)
            {
                var deviceEvent = new DeviceEvent()
                {
                    TimestampOnUtc = timestamp,
                    DeviceId = device.Id,
                    TrackId = track.Id,
                    Event = EventType.LowLevelBattery,
                    DateOnUtc = dateOnUtc,
                };
                _deviceEventRepository.Insert(deviceEvent);
            }
            if (deviceSettings.ControlButtonSos && alarm)
            {
                var deviceEvent = new DeviceEvent()
                {
                    TimestampOnUtc = timestamp,
                    DeviceId = device.Id,
                    TrackId = track.Id,
                    Event = EventType.SOS,
                    DateOnUtc = dateOnUtc,
                };
                _deviceEventRepository.Insert(deviceEvent);
            }
            #endregion Device events
            #region Set indicators
            if (!deviceSettings.ControlSatellites && !deviceSettings.ControlBattery && !deviceSettings.ControlGSM) return true;

            var deviceIndicator = new DeviceIndicator();

            if (deviceSettings.ControlSatellites)
                deviceIndicator.Satellites = 8; // data.Satellites;
            if (deviceSettings.ControlBattery)
                deviceIndicator.Battery = 50; // data.Battery ?? 0;
            if (deviceSettings.ControlGSM)
                deviceIndicator.GSM = 28; // data.GSM ?? 0;

            deviceIndicator.TimestampOnUtc = timestamp;
            deviceIndicator.DeviceId = device.Id;
            deviceIndicator.TrackId = track.Id;
            deviceIndicator.DateOnUtc = dateOnUtc;
            deviceIndicator.ExtendedInfo = string.Empty; // data.ExtendedInfo;

            _deviceIndicatorRepository.Insert(deviceIndicator);
            #endregion Set indicators
            
            return true;
        }
        #endregion Location

        #region DevicePosition
        public void InsertDevicePosition(DevicePosition devicePosition)
        {
            if (devicePosition == null)
                throw new ArgumentNullException("devicePosition");

            _devicePositionRepository.Insert(devicePosition);

            //cache
            //_cacheManager.RemoveByPattern(DEVICES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityInserted(device);
        }
        public virtual void UpdateDevicePosition(DevicePosition devicePosition)
        {
            if (devicePosition == null)
                throw new ArgumentNullException("devicePosition");

            //update device
            _devicePositionRepository.Update(devicePosition);

            //cache
            //_cacheManager.RemoveByPattern(DEVICES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityUpdated(device);
        }
        public void DeleteDevicePosition(DevicePosition devicePosition)
        {
            if (devicePosition == null)
                throw new ArgumentNullException("devicePosition");

            _devicePositionRepository.Delete(devicePosition);

            //cache
            //_cacheManager.RemoveByPattern(DEVICES_PATTERN_KEY);

            //event notification
            //_eventPublisher.EntityDeleted(device);
        }
        #endregion DevicePosition

        #region DeviceSetting
        public virtual void UpdateDeviceSetting(DeviceSetting deviceSetting)
        {
            if (deviceSetting == null)
                throw new ArgumentNullException("deviceSetting");

            //update language
            _deviceSettingRepository.Update(deviceSetting);

            //cache
            _cacheManager.RemoveByPattern(DEVICESETTING_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(deviceSetting);
        }
        public void InsertDeviceSetting(DeviceSetting deviceSetting)
        {
            if (deviceSetting == null)
                throw new ArgumentNullException("deviceSetting");

            _deviceSettingRepository.Insert(deviceSetting);

            //cache
            _cacheManager.RemoveByPattern(DEVICESETTING_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(deviceSetting);
        }
        public void DeleteDeviceSetting(DeviceSetting deviceSetting)
        {
            if (deviceSetting == null)
                throw new ArgumentNullException("deviceSetting");

            _deviceSettingRepository.Delete(deviceSetting);

            //cache
            _cacheManager.RemoveByPattern(DEVICESETTING_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(deviceSetting);
        }
        #endregion DeviceSetting

        #region Device type
        public DeviceType GetDeviceType(string factory, string deviceModel)
        {
            var query = _deviceTypeRepository.Table;
            if (!factory.IsNullOrEmpty())
                query = query.Where(d => d.Factory == factory);
            if (!deviceModel.IsNullOrEmpty())
                query = query.Where(d => d.Model == deviceModel);

            return query.FirstOrDefault();
        }
        public IList<DeviceType> GetAllDeviceTypes()
        {
            var query = _deviceTypeRepository.Table;
            query = query.OrderBy(c => c.Factory);

            return query.ToList();
        }
        public IPagedList<DeviceType> GetAllDeviceTypes(int pageIndex, int pageSize)
        {
            var query = _deviceTypeRepository.Table;
            query = query.OrderBy(c => c.Factory);

            var deviceType = new PagedList<DeviceType>(query, pageIndex, pageSize);
            return deviceType;
        }
        public DeviceType GetDeviceTypeById(long deviceTypeId)
        {
            if (deviceTypeId == 0)
                return null;

            var deviceType = _deviceTypeRepository.GetById(deviceTypeId);
            return deviceType;
        }
        public DeviceType GetDeviceTypeByDevice(Device device)
        {
            return device.DeviceType;
        }
        public void InsertDeviceType(DeviceType deviceType)
        {
            if (deviceType == null)
                throw new ArgumentNullException("deviceType");

            _deviceTypeRepository.Insert(deviceType);

            //cache
            _cacheManager.RemoveByPattern(DEVICESTYPE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(deviceType);
        }
        public virtual void UpdateDeviceType(DeviceType deviceType)
        {
            if (deviceType == null)
                throw new ArgumentNullException("deviceType");

            //update language
            _deviceTypeRepository.Update(deviceType);

            //cache
            _cacheManager.RemoveByPattern(DEVICESTYPE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(deviceType);
        }
        public virtual void DeleteDeviceType(DeviceType deviceType)
        {
            if (deviceType == null)
                throw new ArgumentNullException("deviceType");

            //update language
            _deviceTypeRepository.Delete(deviceType);

            //cache
            _cacheManager.RemoveByPattern(DEVICESTYPE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(deviceType);
        }
        #endregion Device type

        public void InsertDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            _deviceRepository.Insert(device);

            //cache
            _cacheManager.RemoveByPattern(DEVICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(device);
        }
        public virtual void UpdateDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            //update device
            _deviceRepository.Update(device);

            //cache
            _cacheManager.RemoveByPattern(DEVICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(device);
        }
        public void DeleteDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            _deviceRepository.Delete(device);

            //cache
            _cacheManager.RemoveByPattern(DEVICES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(device);
        }
        #endregion Methods
    }
}
