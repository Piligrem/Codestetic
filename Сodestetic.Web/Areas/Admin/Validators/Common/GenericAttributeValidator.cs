using FluentValidation;
using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Services.Localization;

namespace Codestetic.Web.Areas.Admin.Validators.Localization
{
    public class GenericAttributeValidator : AbstractValidator<GenericAttributeModel>
    {
        public GenericAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Key).NotEmpty().WithMessage(localizationService.GetResource("Admin.Common.GenericAttributes.Fields.Name.Required"));
        }
    }
}