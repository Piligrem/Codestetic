using FluentValidation;
using Codestetic.Web.Areas.Admin.Models.Settings;
using Codestetic.Web.Services.Localization;

namespace Codestetic.Web.Areas.Admin.Validators.Settings
{
    public class GeneralCommonSettingsValidator : AbstractValidator<GeneralCommonSettingsModel>
    {
        public GeneralCommonSettingsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ContactDataSettings.CompanyEmailAddress)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.ContactDataSettings.ContactEmailAddress)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.ContactDataSettings.SupportEmailAddress)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.ContactDataSettings.WebmasterEmailAddress)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
        }
    }
}