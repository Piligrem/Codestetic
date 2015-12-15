using FluentValidation;
using Codestetic.Web.Areas.Admin.Models.Settings;
using Codestetic.Web.Services.Localization;

namespace Codestetic.Web.Areas.Admin.Validators.Settings
{
    public class SettingValidator : AbstractValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));
        }
    }
}