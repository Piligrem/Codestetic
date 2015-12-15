using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Codestetic.Web.Validators.Users;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Users
{
    [Validator(typeof(UserValidator))]
    public class UserModel : EntityModelBase
    {
        public UserModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            SendEmail = new SendEmailModel();
            SendPm = new SendPmModel();
            AvailableUserRoles = new List<UserRoleModel>();
            AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            AvailableCountries = new List<SelectListItem>();
        }

        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }

        [ResourceDisplayName("Admin.Users.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [ResourceDisplayName("Admin.Users.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [ResourceDisplayName("Admin.Users.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [ResourceDisplayName("Common.Fields.Gender")]
        public string Gender { get; set; }

        [ResourceDisplayName("Admin.Users.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.FullName")]
        public string FullName { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.DateOfBirth")]
        [DisplayFormat(NullDisplayText = "", DataFormatString = "0:MM/dd/yyyy")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.Country")]
        public long CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool PhoneEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }

        [ResourceDisplayName("Admin.Users.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [ResourceDisplayName("Admin.Users.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [ResourceDisplayName("Admin.Users.Advance.Fields.Active")]
        public bool Active { get; set; }

        //time zone
        [ResourceDisplayName("Admin.Users.Fields.TimeZoneId")]
        [AllowHtml]
        public string TimeZoneId { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [ResourceDisplayName("Admin.Users.Fields.VatNumber")]
        [AllowHtml]
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }

        //registration date
        [ResourceDisplayName("Admin.Users.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [ResourceDisplayName("Admin.Users.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        //IP address
        [ResourceDisplayName("Admin.Users.Fields.IPAddress")]
        public string LastIpAddress { get; set; }


        [ResourceDisplayName("Admin.Users.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }

        //user roles
        [ResourceDisplayName("Admin.Users.Advance.Fields.UserRoles")]
        public string UserRoleNames { get; set; }
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public long[] SelectedUserRoleIds { get; set; }
        public bool AllowManagingUserRoles { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }
        //send PM model
        public SendPmModel SendPm { get; set; }

        [ResourceDisplayName("Admin.Users.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }


        #region Nested classes
        public class AssociatedExternalAuthModel : EntityModelBase
        {
            [ResourceDisplayName("Admin.Users.Advance.AssociatedExternalAuth.Fields.Email")]
            public string Email { get; set; }

            [ResourceDisplayName("Admin.Users.Advance.AssociatedExternalAuth.Fields.ExternalIdentifier")]
            public string ExternalIdentifier { get; set; }

            [ResourceDisplayName("Admin.Users.Advance.AssociatedExternalAuth.Fields.AuthMethodName")]
            public string AuthMethodName { get; set; }
        }

        public class SendEmailModel : ModelBase
        {
            [ResourceDisplayName("Admin.Users.Advance.SendEmail.Subject")]
            [AllowHtml]
            public string Subject { get; set; }

            [ResourceDisplayName("Admin.Users.Advance.SendEmail.Body")]
            [AllowHtml]
            public string Body { get; set; }
        }

        public class SendPmModel : ModelBase
        {
            [ResourceDisplayName("Admin.Users.Advance.SendPM.Subject")]
            public string Subject { get; set; }

            [ResourceDisplayName("Admin.Users.Advance.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial class ActivityLogModel : EntityModelBase
        {
            [ResourceDisplayName("Admin.Users.Advance.ActivityLog.ActivityLogType")]
            public string ActivityLogTypeName { get; set; }
            [ResourceDisplayName("Admin.Users.Advance.ActivityLog.Comment")]
            public string Comment { get; set; }
            [ResourceDisplayName("Admin.Users.Advance.ActivityLog.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }
        #endregion
    }
}
