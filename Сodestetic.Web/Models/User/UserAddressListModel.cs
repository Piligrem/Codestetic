using System.Collections.Generic;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Areas.Admin.Models.Common;

namespace Codestetic.Web.Models.User
{
    public partial class UserAddressListModel : ModelBase
    {
        public UserAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}