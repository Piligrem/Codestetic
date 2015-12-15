using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Seo;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Areas.Admin.Validators.Settings;
using Codestetic.Web.Core.Domain.Localization;

namespace Codestetic.Web.Areas.Admin.Models.Settings
{
    [Validator(typeof(GeneralCommonSettingsValidator))]
	public partial class GeneralCommonSettingsModel : ModelBase
    {
        public GeneralCommonSettingsModel()
        {
            SiteInformationSettings = new SiteInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            PdfSettings = new PdfSettingsModel();
            LocalizationSettings = new LocalizationSettingsModel(); 
            FullTextSettings = new FullTextSettingsModel();
            CompanyInformationSettings = new CompanyInformationSettingsModel();
            ContactDataSettings = new ContactDataSettingsModel();
            BankConnectionSettings = new BankConnectionSettingsModel();
            SocialSettings = new SocialSettingsModel();
        }

        public SiteInformationSettingsModel SiteInformationSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        public PdfSettingsModel PdfSettings { get; set; }
        public LocalizationSettingsModel LocalizationSettings { get; set; }
        public FullTextSettingsModel FullTextSettings { get; set; }
        public CompanyInformationSettingsModel CompanyInformationSettings { get; set; }
        public ContactDataSettingsModel ContactDataSettings { get; set; }
        public BankConnectionSettingsModel BankConnectionSettings { get; set; }
        public SocialSettingsModel SocialSettings { get; set; }

        #region Nested classes

		public partial class SiteInformationSettingsModel
        {
            public SiteInformationSettingsModel()
            {
            }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SiteClosed")]
			public bool SiteClosed { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SiteClosedAllowForAdmins")]
            public bool SiteClosedAllowForAdmins { get; set; }
        }

        public partial class SeoSettingsModel
        {
            public SeoSettingsModel()
            {
                AvailablePageTitleSeoAdjustment = new List<SelectListItem>();
            }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            public string PageTitleSeparator { get; set; }

            public IList<SelectListItem> AvailablePageTitleSeoAdjustment { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PageTitleSeoAdjustment")]
            public string PageTitleSeoAdjustment { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords")]
            [AllowHtml]
            public string DefaultMetaKeywords { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription")]
            [AllowHtml]
            public string DefaultMetaDescription { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ConvertNonWesternChars")]
            public bool ConvertNonWesternChars { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CanonicalUrlsEnabled")]
            public bool CanonicalUrlsEnabled { get; set; }
        }

		public partial class SecuritySettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
            [AllowHtml]
            public string AdminAreaAllowedIpAddresses { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HideAdminMenuItemsBasedOnPermissions")]
            public bool HideAdminMenuItemsBasedOnPermissions { get; set; }


            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabled")]
            public bool CaptchaEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnLoginPage")]
            public bool CaptchaShowOnLoginPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnRegistrationPage")]
            public bool CaptchaShowOnRegistrationPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnContactUsPage")]
            public bool CaptchaShowOnContactUsPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailWishlistToFriendPage")]
            public bool CaptchaShowOnEmailWishlistToFriendPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnEmailProductToFriendPage")]
            public bool CaptchaShowOnEmailProductToFriendPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnAskQuestionPage")]
            public bool CaptchaShowOnAskQuestionPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnBlogCommentPage")]
            public bool CaptchaShowOnBlogCommentPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnNewsCommentPage")]
            public bool CaptchaShowOnNewsCommentPage { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnProductReviewPage")]
            public bool CaptchaShowOnProductReviewPage { get; set; }
            
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPublicKey")]
            [AllowHtml]
            public string ReCaptchaPublicKey { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.reCaptchaPrivateKey")]
            [AllowHtml]
            public string ReCaptchaPrivateKey { get; set; }
        }

		public partial class PdfSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfEnabled")]
            public bool Enabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLetterPageSizeEnabled")]
            public bool LetterPageSizeEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.PdfLogo")]
            [UIHint("Picture")]
            public long LogoPictureId { get; set; }
        }

		public partial class LocalizationSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.UseImagesForLanguageSelection")]
            public bool UseImagesForLanguageSelection { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled")]
            public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup")]
            public bool LoadAllLocaleRecordsOnStartup { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DefaultLanguageRedirectBehaviour")]
            public DefaultLanguageRedirectBehaviour DefaultLanguageRedirectBehaviour { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InvalidLanguageRedirectBehaviour")]
            public InvalidLanguageRedirectBehaviour InvalidLanguageRedirectBehaviour { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DetectBrowserUserLanguage")]
            public bool DetectBrowserUserLanguage { get; set; }
        }

		public partial class FullTextSettingsModel
        {
            public bool Supported { get; set; }

            public bool Enabled { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.SearchMode")]
            public FulltextSearchMode SearchMode { get; set; }
            public SelectList SearchModeValues { get; set; }
        }

		public partial class CompanyInformationSettingsModel
        {

            public CompanyInformationSettingsModel()
            {
                AvailableCountries = new List<SelectListItem>();
                Salutations = new List<SelectListItem>();
                ManagementDescriptions = new List<SelectListItem>();
            }

            public IList<SelectListItem> AvailableCountries { get; set; }
            public IList<SelectListItem> Salutations { get; set; }
            public IList<SelectListItem> ManagementDescriptions { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CompanyName")]
            public string CompanyName { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Salutation")]
            public string Salutation { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Title")]
            public string Title { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Firstname")]
            public string Firstname { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Lastname")]
            public string Lastname { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CompanyManagementDescription")]
            public string CompanyManagementDescription { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CompanyManagement")]
            public string CompanyManagement { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Street")]
            public string Street { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Street2")]
            public string Street2 { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ZipCode")]
            public string ZipCode { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Location")]
            public string City { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Country")]
            public int CountryId { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.Country")]
            [AllowHtml]
            public string CountryName { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.State")]
            public string Region { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.VatId")]
            public string VatId { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.CommercialRegister")]
            public string CommercialRegister { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.TaxNumber")]
            public string TaxNumber { get; set; }
        }

		public partial class ContactDataSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.CompanyTelephoneNumber")]
            public string CompanyTelephoneNumber { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.HotlineTelephoneNumber")]
            public string HotlineTelephoneNumber { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.MobileTelephoneNumber")]
            public string MobileTelephoneNumber { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.CompanyFaxNumber")]
            public string CompanyFaxNumber { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.CompanyEmailAddress")]
            public string CompanyEmailAddress { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.WebmasterEmailAddress")]
            public string WebmasterEmailAddress { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.SupportEmailAddress")]
            public string SupportEmailAddress { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.ContactDataSettings.ContactEmailAddress")]
            public string ContactEmailAddress { get; set; }
        }

		public partial class BankConnectionSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Bankname")]
            public string Bankname { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Bankcode")]
            public string Bankcode { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.AccountNumber")]
            public string AccountNumber { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.AccountHolder")]
            public string AccountHolder { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Iban")]
            public string Iban { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.BankConnectionSettings.Bic")]
            public string Bic { get; set; }
        }

		public partial class SocialSettingsModel
        {
            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.ShowSocialLinksInFooter")]
            public bool ShowSocialLinksInFooter { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.FacebookLink")]
            public string FacebookLink { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.GooglePlusLink")]
            public string GooglePlusLink { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.TwitterLink")]
            public string TwitterLink { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.PinterestLink")]
            public string PinterestLink { get; set; }

            [ResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SocialSettings.YoutubeLink")]
            public string YoutubeLink { get; set; }
        }
        #endregion
    }
}