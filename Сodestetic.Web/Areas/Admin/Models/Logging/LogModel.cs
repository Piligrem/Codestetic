using System;
using System.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Logging
{
    public class LogModel : EntityModelBase
    {
        public string LogLevelHint { get; set; }
        
        [ResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        [AllowHtml]
        public string ShortMessage { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        [AllowHtml]
        public string FullMessage { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
        [AllowHtml]
        public string IpAddress { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.User")]
        public long? UserId { get; set; }
        [ResourceDisplayName("Admin.System.Log.Fields.User")]
        public string UserEmail { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        [AllowHtml]
        public string PageUrl { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        [AllowHtml]
        public string ReferrerUrl { get; set; }

        [ResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

		[ResourceDisplayName("Admin.System.Log.Fields.UpdatedOn")]
		public DateTime? UpdatedOn { get; set; }

		[ResourceDisplayName("Admin.System.Log.Fields.Frequency")]
		public int Frequency { get; set; }

		[ResourceDisplayName("Admin.System.Log.Fields.ContentHash")]
		public string ContentHash { get; set; }
    }
}