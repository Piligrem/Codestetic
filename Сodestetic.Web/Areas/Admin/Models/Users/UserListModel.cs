using System.Collections.Generic;
using System.Web.Mvc;
//using Telerik.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Areas.Admin.Infrastructure;

namespace Codestetic.Web.Areas.Admin.Models.Users
{
    public class UserListModel : ModelBase
    {
        public GridModel<UserModel> Users { get; set; }

        [ResourceDisplayName("Admin.Users.List.UserRoles")]
        [AllowHtml]
        public List<UserRoleModel> AvailableUserRoles { get; set; }

        [ResourceDisplayName("Admin.Users.List.UserRoles")]
        public long[] SearchUserRoleIds { get; set; }

        [ResourceDisplayName("Admin.Users.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [ResourceDisplayName("Admin.Users.List.SearchUsername")]
        [AllowHtml]
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [ResourceDisplayName("Admin.Users.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }
        [ResourceDisplayName("Admin.Users.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }
        
        [ResourceDisplayName("Admin.Users.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchDayOfBirth { get; set; }
        [ResourceDisplayName("Admin.Users.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchMonthOfBirth { get; set; }
        public bool DateOfBirthEnabled { get; set; }
        
        [ResourceDisplayName("Admin.Users.List.SearchCompany")]
        [AllowHtml]
        public string SearchCompany { get; set; }
        public bool CompanyEnabled { get; set; }

        [ResourceDisplayName("Admin.Users.List.SearchPhone")]
        [AllowHtml]
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [ResourceDisplayName("Admin.Users.List.SearchZipCode")]
        [AllowHtml]
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }
    }
}
