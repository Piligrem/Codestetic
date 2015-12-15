using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Logging
{
    public class LogSearchListModel : ModelBase
    {
        public LogSearchListModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        [ResourceDisplayName("Admin.System.Log.List.CreatedOnFrom")]
        public DateTime? CreatedOnFrom { get; set; }

        [ResourceDisplayName("Admin.System.Log.List.CreatedOnTo")]
        public DateTime? CreatedOnTo { get; set; }

        [ResourceDisplayName("Admin.System.Log.List.Message")]
        [AllowHtml]
        public string Message { get; set; }

        [ResourceDisplayName("Admin.System.Log.List.LogLevel")]
        public int LogLevelId { get; set; }

		[ResourceDisplayName("Admin.System.Log.List.MinFrequency")]
		public int MinFrequency { get; set; }

        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}