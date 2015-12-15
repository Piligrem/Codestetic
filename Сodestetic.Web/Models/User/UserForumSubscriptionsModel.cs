using System.Collections.Generic;
using Codestetic.Web.Core;

namespace Codestetic.Web.Models.User
{
    public partial class UserForumSubscriptionsModel : PagedListBase
    {
        public UserForumSubscriptionsModel(IPageable pageable) : base(pageable)
        {
            this.ForumSubscriptions = new List<ForumSubscriptionModel>();
        }

        public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}