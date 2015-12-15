using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.User
{
    public partial class UserAddressEditModel : ModelBase
    {
        public UserAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
        public UserNavigationModel NavigationModel { get; set; }
    }
}