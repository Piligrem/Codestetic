using FluentValidation;
using Codestetic.Web.Areas.Admin.Models.Users;
using Codestetic.Web.Services.Localization;

namespace Codestetic.Web.Areas.Admin.Validators.User
{
    public class UserRoleValidator : AbstractValidator<UserRoleModel>
    {
        public UserRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Users.UserRoles.Fields.Name.Required"));
        }
    }
}
