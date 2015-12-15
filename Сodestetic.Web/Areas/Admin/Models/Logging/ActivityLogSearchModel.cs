using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Logging
{
    public class ActivityLogSearchModel : ModelBase
    {
        public ActivityLogSearchModel()
        {
            ActivityLogType = new List<SelectListItem>();
        }
        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.CreatedOnFrom")]
        public DateTime? CreatedOnFrom { get; set; }

        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.CreatedOnTo")]
        public DateTime? CreatedOnTo { get; set; }

        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.UserEmail")]
        [AllowHtml]
        public string UserEmail { get; set; }

        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.ActivityLogType")]
        public int ActivityLogTypeId { get; set; }

        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.ActivityLogType")]
        public IList<SelectListItem> ActivityLogType { get; set; }
    }
}