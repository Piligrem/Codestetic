using FluentValidation;
using Codestetic.Web.Areas.Admin.Models.Directory;
using Codestetic.Web.Areas.Admin.Models.Tasks;
using Codestetic.Web.Services.Localization;

namespace Codestetic.Web.Areas.Admin.Validators.Tasks
{
    public class ScheduleTaskValidator : AbstractValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Seconds.Positive"));
        }
    }
}