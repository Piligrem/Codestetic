using System.Web.Mvc;
using FluentValidation.Attributes;
using Codestetic.Web.Areas.Admin.Validators.Localization;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Common
{
    [Validator(typeof(GenericAttributeValidator))]
    public partial class GenericAttributeModel : EntityModelBase
    {
        [ResourceDisplayName("Admin.Common.GenericAttributes.Fields.Name")]
        public string Key { get; set; }

        [AllowHtml]
        [ResourceDisplayName("Admin.Common.GenericAttributes.Fields.Value")]
        public string Value { get; set; }

        public string EntityName { get; set; }

        public long EntityId { get; set; }
    }
}