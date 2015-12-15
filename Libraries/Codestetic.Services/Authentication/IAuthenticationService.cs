using Codestetic.Web.Core.Domain.Users;

namespace Codestetic.Web.Services.Authentication
{
    public partial interface IAuthenticationService
    {
        void SignIn(User user, bool createPersistentCookie);
        void SignOut();
        User GetAuthenticatedUser();
    }
}
