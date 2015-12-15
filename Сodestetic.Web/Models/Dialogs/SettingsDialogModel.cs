using System.ComponentModel.DataAnnotations;
using Codestetic.Web.Framework.Mvc;
using System.Collections.Generic;
using Codestetic.Web.Core.Domain.Tasks;
using Codestetic.Web.Models.Notices;
using System.Web.Mvc;

namespace Codestetic.Web.Models.Dialogs
{
    public class SettingsDialogModel : DialogModelBase
    {
        public SettingsDialogModel()
        {
            Tasks = new List<SelectListItem>();
            Notifies = new List<NotifyModel>();
        }

        public string Name { get; set; }
        public long DeviceId { get; set; }
        [UIHint("Photo")]
        public long PhotoId { get; set; }
        [UIHint("Slider")]
        public long MarkerId { get; set; }
        public bool ToleranceEnable { get; set; }
        public int Tolerance { get; set; }
        public string StrokeColor { get; set; }
        public int StrokeWidth { get; set; }
        public string FillColor { get; set; }
        public bool TrackEnable { get; set; }
        public string TrackStrokeColor { get; set; }
        public int TrackStrokeWidth { get; set; }
        public int IntervalUpdateDevice { get; set; }
        public bool ControlSatellites { get; set; }
        public bool ControlBattery { get; set; }
        public int MinBatteryLevel { get; set; }
        public bool ControlGSM { get; set; }
        public bool ControlButtonSos { get; set; }
        public bool ControlInGeoZone { get; set; }
        public bool ControlOutGeoZone { get; set; }

        #region Notify
        public IList<SelectListItem> Tasks { get; set; }
        public IList<NotifyModel> Notifies { get; set; }
        #endregion Notify
    }
}