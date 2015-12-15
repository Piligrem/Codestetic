using Codestetic.Web.Core.Domain.GeoZones;
using Codestetic.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Models.Setting
{
    public class DefaultSettingsModel : ModelBase
    {
        #region Constructors
        public DefaultSettingsModel() {
            GeoZone = new GeoZoneSettingModel();
        }
        #endregion Constructors

        #region Properties
        public GeoZoneSettingModel GeoZone { get; set; }
        #endregion Properties
    }

    #region Additional classes
    public class GeoZoneSettingModel
    {
        public string StrokeColor { get; set; }
        public int StrokeWidth { get; set; }
        public string FillColor { get; set; }
        public string StrokeDash { get; set; }
        public string DrawErrorColor { get; set; }
        public int DrawErrorTimeout { get; set; }
    }
    #endregion Additional classes
}