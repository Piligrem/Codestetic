using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.User
{
    public partial class UserAvatarModel : ModelBase
    {
        public string AvatarUrl { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}