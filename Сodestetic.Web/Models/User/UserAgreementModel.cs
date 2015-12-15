using System;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.User
{
    public partial class UserAgreementModel : ModelBase
    {
        public Guid OrderItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}