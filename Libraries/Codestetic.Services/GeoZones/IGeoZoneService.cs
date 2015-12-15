using System;
using System.Collections.Generic;
using Codestetic.Web.Core.Domain.Devices;
using System.Data.Entity.Spatial;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.GeoZones;

namespace Codestetic.Web.Services.GeoZones
{
    public interface IGeoZoneService
    {

        IList<GeoZone> GetGeoZonesByDeviceId(long deviceId);
        IList<GeoZone> GetGeoZonesByDevice(Device device);
        IList<GeoZone> GetGeoZonesByUser(User user);
        GeoZone GetGeoZoneById(long id);
        GeoZone GetGeoZoneByName(string name);

        void SaveGeoZone(GeoZone geoZone);
        void InsertGeoZone(GeoZone geoZone);
        void UpdateGeoZone(GeoZone geoZone);
        void DeleteGeoZone(GeoZone geoZone);
        void DeleteGeoZoneByIds(IList<long> ids);
    }
}
