using System.Web.Mvc;
using FluentValidation.Attributes;
using Codestetic.Web.Areas.Admin.Validators.Settings;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Settings
{
    [Validator(typeof(SettingValidator))]
    public class SettingModel : EntityModelBase
    {
        [ResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }
    }
}