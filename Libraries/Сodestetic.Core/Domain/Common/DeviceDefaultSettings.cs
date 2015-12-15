using Specter.Web.Core.Configuration;

namespace Specter.Web.Core.Domain.Common
{
    public class DeviceDefaultSettings : ISettings
    {
        public DeviceDefaultSettings() { }

        public bool ToleranceEnable { get; set; }
        public int ToleranceRadius { get; set; }
        public string ToleranceStrokeColor { get; set; }
        public int ToleranceStrokeWidth { get; set; }
        public string ToleranceFillColor { get; set; }

        public bool TrackEnable { get; set; }
        public string TrackStrokeColor { get; set; }
        public int TrackStrokeWidth { get; set; }

        public string GeoZoneStrokeColor { get; set; }
        public int GeoZoneStrokeWidth { get; set; }
        public string GeoZoneFillColor { get; set; }
        public string GeoZoneStrokeDash { get; set; }
        public string GeoZoneDrawErrorColor { get; set; }
        public int GeoZoneDrawErrorTimeout { get; set; }

        public int IntervalUpdateDevice { get; set; }

        public bool ControlSatellites { get; set; }
        public bool ControlBattery { get; set; }
        public double MinBatteryLevel { get; set; }
        public bool ControlGSM { get; set; }
        public bool ControlButtonSos { get; set; }
        public bool ControlInGeoZone { get; set; }
        public bool ControlOutGeoZone { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}