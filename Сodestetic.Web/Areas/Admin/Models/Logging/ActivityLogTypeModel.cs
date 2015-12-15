using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Logging
{
    public class ActivityLogTypeModel : EntityModelBase
    {
        [ResourceDisplayName("Admin.Configuration.ActivityLogType.Fields.Name")]
        public string Name { get; set; }
        [ResourceDisplayName("Admin.Configuration.ActivityLogType.Fields.Enabled")]
        public bool Enabled { get; set; }
    }
}