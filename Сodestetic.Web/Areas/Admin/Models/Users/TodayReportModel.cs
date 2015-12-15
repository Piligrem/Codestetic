using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Users
{
    public class TodayReportModel : ModelBase
    {
        [ResourceDisplayName("Admin.Users.Reports.TodayReport.Fields.NowConnected")]
        public int NowConnectedDevices { get; set; }
        [ResourceDisplayName("Admin.Users.Reports.TodayReport.Fields.TodayConnected")]
        public int TodayConnected { get; set; }
        [ResourceDisplayName("Admin.Users.Reports.TodayReport.Fields.Active")]
        public int TodayActiveDevices { get; set; }
        [ResourceDisplayName("Admin.Users.Reports.TodayReport.Fields.Deactivated")]
        public int TodayDeactivatedDevices { get; set; }
        [ResourceDisplayName("Admin.Users.Reports.TodayReport.Fields.UsersOnline")]
        public int UsersOnline { get; set; }
    }
}
