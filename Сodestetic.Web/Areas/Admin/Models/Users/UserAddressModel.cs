using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Users
{
    public class UserAddressModel : ModelBase
    {
        public long UserId { get; set; }

        public AddressModel Address { get; set; }
    }
}
