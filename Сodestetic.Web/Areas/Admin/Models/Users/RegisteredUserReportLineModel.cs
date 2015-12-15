using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Users
{
    public class RegisteredUserReportLineModel : ModelBase
    {
        [ResourceDisplayName("Admin.Users.Reports.RegisteredUsers.Fields.Period")]
        public string Period { get; set; }

        [ResourceDisplayName("Admin.Users.Reports.RegisteredUsers.Fields.Users")]
        public int Users { get; set; }
    }
}
