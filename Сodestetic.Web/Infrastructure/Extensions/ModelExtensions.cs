using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Services.Media;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Core.Domain.GeoZones;
using Codestetic.Web.Core.Domain.Common;

namespace Codestetic.Web
{
    public static class ModelExtensions
    {
        private static readonly IPictureService _pictureService = EngineContext.Current.Resolve<IPictureService>();

        #region Merker extensions
        public static object IconModel(this Marker marker, long id = 0)
        {
            if (id != 0)
            {
                marker = _pictureService.GetMarkerById(id);
            }
            return new
            {
                id = marker.Id,
                url = _pictureService.GetMarkerUrl(marker.Id),
                width = marker.Width,
                height = marker.Height,
                x = marker.AnchorX,
                y = marker.AnchorY
            };
        }
        public static object ToModel(this Marker marker) 
        {
            var result = new
            {
                marker = marker.IconModel(),
                shadow = marker.ShadowMarkerId != null ? marker.IconModel(marker.ShadowMarkerId.Value) : null
            };
            return result;
        }
        #endregion Merker extensions

        #region Tolerance and Track extensions
        public static object ToModel(this DeviceSetting setting)
        {
            return new
            {
                marker = setting.Marker.ToModel(),
                tolerance = new
                {
                    enable = setting.ToleranceEnable,
                    radius = setting.ToleranceRadius,
                    strokeWidth = setting.ToleranceStrokeWidth,
                    strokeColor = setting.ToleranceStrokeColor,
                    fillColor = setting.ToleranceFillColor
                },
                track = new
                {
                    enable = setting.TrackEnable,
                    strokeWidth = setting.TrackStrokeWidth,
                    strokeColor = setting.TrackStrokeColor,
                    length = setting.TrackLength
                }
            };
        }
        #endregion Tolerance extensions

        #region GeoZone extensions
        public static object ToModel(this GeoZone zone)
        {
            return new
            {
                id = zone.Id,
                name = zone.Name,
                strokeColor = zone.StrokeColor,
                strokeWidth = zone.StrokeWidth,
                fillColor = zone.FillColor,
                prototype = zone.PrototypeZone.ToString(),
                coordinates = Geo.Parse<List<List<List<double>>>>(zone.Zone)
            };
        }
        #endregion GeoZone extensions

        #region Default settings
        public static object ToModel(this DeviceDefaultSettings settings) 
        {
            return new
            {
                strokeColor = settings.GeoZoneStrokeColor,
                strokeWidth = settings.GeoZoneStrokeWidth,
                fillColor = settings.GeoZoneFillColor,
                strokeDash = settings.GeoZoneStrokeDash,
                drawErrorColor = settings.GeoZoneDrawErrorColor,
                drawErrorTimeout = settings.GeoZoneDrawErrorTimeout
            };
        }
        #endregion Default settings
    }
}