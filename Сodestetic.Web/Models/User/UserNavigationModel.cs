using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.User
{
    public partial class UserNavigationModel : ModelBase
    {
        public bool HideInfo { get; set; }
        public bool HideAddresses { get; set; }
        public bool HideOrders { get; set; }
        public bool HideChangePassword { get; set; }
        public bool HideAvatar { get; set; }
        public bool HideForumSubscriptions { get; set; }

        public UserNavigationEnum SelectedTab { get; set; }
    }

    public enum UserNavigationEnum
    {
        Info,
        Addresses,
        Orders,
        BackInStockSubscriptions,
        ChangePassword,
        Avatar,
        ForumSubscriptions
    }
}