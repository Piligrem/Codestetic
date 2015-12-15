using FluentValidation;
using Codestetic.Web.Areas.Admin.Models.ContentSlider;
using Codestetic.Web.Services.Localization;

namespace Codestetic.Web.Areas.Admin.Validators.ContentSlider
{
    public class ContentSliderSlideValidator : AbstractValidator<ContentSliderSlideModel>
    {
        public ContentSliderSlideValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentSlider.Slide.Title.Required"));
        }
    }

    public class ContentSliderButtonValidator : AbstractValidator<ContentSliderButtonModel>
    {
        public ContentSliderButtonValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Url)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentSlider.Slide.ButtonUrl.Required"))
                .When(x => x.Published);
            RuleFor(x => x.Text)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentSlider.Slide.ButtonText.Required"))
                .When(x => x.Published);
        }
    }
}