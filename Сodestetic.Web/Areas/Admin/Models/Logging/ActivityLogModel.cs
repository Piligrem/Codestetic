using System;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Logging
{
    public partial class ActivityLogModel : EntityModelBase
    {
        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }
        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.User")]
        public int UserId { get; set; }
        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.User")]
        public string UserEmail { get; set; }
        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.Comment")]
        public string Comment { get; set; }
        [ResourceDisplayName("Admin.Configuration.ActivityLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
