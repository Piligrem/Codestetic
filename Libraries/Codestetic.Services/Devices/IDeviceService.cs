using System.Collections.Generic;
using System;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.GPS;

namespace Codestetic.Web.Services.Devices
{
    public interface IDeviceService
    {
        IPagedList<Device> GetAllDevices(int? pageIndex = null, int? pageSize = null, bool hidden = true);
        IEnumerable<Device> GetAllDevicesByUser(User user);
        int GetDevicesCount(User user);
        Device GetDeviceById(long id);
        IEnumerable<DeviceSetting> GetAllDevicesSettingByUser(User user);
        DeviceSetting GetDeviceSettingByDeviceId(long deviceId);
        DeviceSetting GetDeviceSettingById(long id);
        IList<Device> GetDevicesByGeoZoneId(long id);
        Device GetDeviceByIMEI(string imei);

        void AddDevice(User user, string name, string imei, long deviceTypeId);

        DeviceType GetDeviceType(string factory, string deviceModel);
        IList<DeviceType> GetAllDeviceTypes();
        IPagedList<DeviceType> GetAllDeviceTypes(int pageIndex, int pageSize);
        DeviceType GetDeviceTypeById(long deviceTypeId);
        DeviceType GetDeviceTypeByDevice(Device device);
        void InsertDeviceType(DeviceType deviceType);
        void UpdateDeviceType(DeviceType deviceType);
        void DeleteDeviceType(DeviceType deviceType);

        void AddGeoZones(Device device, IList<string> geoZonesNames);
        void RemoveGeoZones(Device device, IList<long> geoZoneIds);

        void UpdateDevice(Device device);
        void InsertDevice(Device device);
        void DeleteDevice(Device device);

        bool SaveLocation(string imei, DateTime dateOnUtc, double latitude, double longitude, double altitude, double angle, double speed, bool alarm);

        void InsertDevicePosition(DevicePosition devicePosition);
        void UpdateDevicePosition(DevicePosition devicePosition);
        void DeleteDevicePosition(DevicePosition devicePosition);

        void InsertDeviceSetting(DeviceSetting deviceSetting);
        void UpdateDeviceSetting(DeviceSetting deviceSetting);
        void DeleteDeviceSetting(DeviceSetting deviceSetting);
    }
}
