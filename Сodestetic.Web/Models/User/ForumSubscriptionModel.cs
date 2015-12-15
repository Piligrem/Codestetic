using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.User
{
    public partial class ForumSubscriptionModel : EntityModelBase
    {
        public long ForumId { get; set; }
        public long ForumTopicId { get; set; }
        public bool TopicSubscription { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}
