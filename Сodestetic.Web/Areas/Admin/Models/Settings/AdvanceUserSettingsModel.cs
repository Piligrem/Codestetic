using System.Collections.Generic;
using System.Web.Mvc;
using Codestetic.Web.Framework;

namespace Codestetic.Web.Areas.Admin.Models.Settings
{
    public partial class AdvanceUserSettingsModel
    {
        public AdvanceUserSettingsModel()
        {
            UserSettings = new UserSettingsModel();
            AddressSettings = new AddressSettingsModel();
            DateTimeSettings = new DateTimeSettingsModel();
            ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
        }
        public UserSettingsModel UserSettings { get; set; }
        public AddressSettingsModel AddressSettings { get; set; }
        public DateTimeSettingsModel DateTimeSettings { get; set; }
        public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

        #region Nested classes

        public partial class UserSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.UsernamesEnabled")]
            public bool UsernamesEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AllowUsersToChangeUsernames")]
            public bool AllowUsersToChangeUsernames { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.CheckUsernameAvailabilityEnabled")]
            public bool CheckUsernameAvailabilityEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.UserRegistrationType")]
            public int UserRegistrationType { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AllowUsersToUploadAvatars")]
            public bool AllowUsersToUploadAvatars { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.DefaultAvatarEnabled")]
            public bool DefaultAvatarEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.ShowUsersLocation")]
            public bool ShowUsersLocation { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.ShowUsersJoinDate")]
            public bool ShowUsersJoinDate { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AllowViewingProfiles")]
            public bool AllowViewingProfiles { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.NotifyNewUserRegistration")]
            public bool NotifyNewUserRegistration { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.HideDownloadableProductsTab")]
            public bool HideDownloadableProductsTab { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.HideBackInStockSubscriptionsTab")]
            public bool HideBackInStockSubscriptionsTab { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.UserNameFormat")]
            public int UserNameFormat { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.DefaultPasswordFormat")]
            public int DefaultPasswordFormat { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.NewsletterEnabled")]
            public bool NewsletterEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.HideNewsletterBlock")]
            public bool HideNewsletterBlock { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.StoreLastVisitedPage")]
            public bool StoreLastVisitedPage { get; set; }


            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.GenderEnabled")]
            public bool GenderEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.DateOfBirthEnabled")]
            public bool DateOfBirthEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.CityEnabled")]
            public bool CityEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.CityRequired")]
            public bool CityRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.CountryEnabled")]
            public bool CountryEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.FaxRequired")]
            public bool FaxRequired { get; set; }
        }

        public partial class AddressSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.CityEnabled")]
            public bool CityEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.CityRequired")]
            public bool CityRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.CountryEnabled")]
            public bool CountryEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AddressFormFields.FaxRequired")]
            public bool FaxRequired { get; set; }
        }

        public partial class DateTimeSettingsModel
        {
            public DateTimeSettingsModel()
            {
                AvailableTimeZones = new List<SelectListItem>();
            }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.AllowUsersToSetTimeZone")]
            public bool AllowUsersToSetTimeZone { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.DefaultStoreTimeZone")]
            public string DefaultStoreTimeZoneId { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.DefaultStoreTimeZone")]
            public IList<SelectListItem> AvailableTimeZones { get; set; }
        }

        public partial class ExternalAuthenticationSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.AdvanceUser.ExternalAuthenticationAutoRegisterEnabled")]
            public bool AutoRegisterEnabled { get; set; }
        }
        #endregion
    }
}