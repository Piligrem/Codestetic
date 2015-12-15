using System.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Common
{
    public partial class UrlRecordModel : EntityModelBase
    {
        [ResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [ResourceDisplayName("Admin.System.SeNames.EntityId")]
        public long EntityId { get; set; }

        [ResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [ResourceDisplayName("Admin.System.SeNames.Active")]
        public bool Active { get; set; }

        [ResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }
    }
}