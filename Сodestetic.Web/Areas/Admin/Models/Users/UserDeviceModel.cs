using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Codestetic.Web.Validators.Users;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Devices
{
    //[Validator(typeof(UserValidator))]
    public class UserDeviceModel : EntityModelBase
    {
        public UserDeviceModel()
        { }

        [ResourceDisplayName("Device.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [ResourceDisplayName("Device.Fields.IMEI")]
        [AllowHtml]
        public string IMEI { get; set; }

        [ResourceDisplayName("Device.Fields.DeviceType")]
        [AllowHtml]
        public string DeviceType { get; set; }

        [ResourceDisplayName("Device.Fields.CreatedOn")]
        //[AllowHtml]
        public DateTime CreatedOn { get; set; }

        [ResourceDisplayName("Device.Fields.UpdateOn")]
        //[AllowHtml]
        public DateTime? UpdateOn { get; set; }

        [ResourceDisplayName("Device.Fields.LastConnection")]
        //[AllowHtml]
        public DateTime? LastConnection { get; set; }

        [ResourceDisplayName("Device.Fields.Active")]
        [AllowHtml]
        public bool Active { get; set; }

        [ResourceDisplayName("Admin.Devices.Fields.DeviceAttached")]
        public bool DeviceAttached { get; set; }
        public string AddButtonStyle { get; set; }
        public string RemoveButtonStyle { get; set; }
    }
}
