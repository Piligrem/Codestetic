using System;
using System.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Logging
{
    public class LogListModel : EntityModelBase
    {
        public string LogLevelHint { get; set; }
        public string ShortMessage { get; set; }
        public string LogLevel { get; set; }
        public DateTime CreatedOn { get; set; }
		public DateTime? UpdatedOn { get; set; }
		public int Frequency { get; set; }
    }
}