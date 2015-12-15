using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models
{
    public partial class SiteHeaderModel : ModelBase
    {
        public string HomeUrl { get; set; }
        public bool LogoUploaded { get; set; }
        public string LogoUrl { get; set; }
        public int LogoWidth { get; set; }
        public int LogoHeight { get; set; }
        public string LogoTitle { get; set; }
        public string UserName { get; set; }
        public bool EnableMenu { get; set; }
    }
}