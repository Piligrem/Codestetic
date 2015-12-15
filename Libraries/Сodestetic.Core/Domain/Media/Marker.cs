using Specter.Web.Core.Domain.Devices;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Specter.Web.Core.Domain.Media
{
    [DataContract]
    public partial class Marker : BaseEntity
    {
        #region Fields
        private ICollection<DeviceSetting> _deviceSettings;
        #endregion Fields

        public byte[] MarkerBinary { get; set; }

        [DataMember]
        public string MimeType { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public bool IsNew { get; set; }

        [DataMember]
        public bool IsSystem { get; set; }

        [DataMember]
        public bool IsRetina { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public double AnchorX { get; set; }

        [DataMember]
        public double AnchorY { get; set; }

        [DataMember]
        public long? ShadowMarkerId { get; set; }

        [DataMember]
        public long? UserId { get; set; }

        #region Entity
        public virtual ICollection<DeviceSetting> DeviceSettings
        {
            get { return _deviceSettings ?? (_deviceSettings = new HashSet<DeviceSetting>()); }
            set { _deviceSettings = value; }
        }
        #endregion Entity
    }
}
