using AutoMapper;
//using Specter.Web.Areas.Admin.Models.Blogs;
//using Specter.Web.Areas.Admin.Models.Catalog;
//using Specter.Web.Areas.Admin.Models.Cms;
using Codestetic.Web.Areas.Admin.Models.ContentSlider;
using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Areas.Admin.Models.Users;
using Codestetic.Web.Areas.Admin.Models.Directory;
//using Specter.Web.Areas.Admin.Models.Discounts;
//using Specter.Web.Areas.Admin.Models.ExternalAuthentication;
//using Specter.Web.Areas.Admin.Models.Forums;
using Codestetic.Web.Areas.Admin.Models.Localization;
using Codestetic.Web.Areas.Admin.Models.Logging;
using Codestetic.Web.Areas.Admin.Models.Messages;
//using Specter.Web.Areas.Admin.Models.News;
//using Specter.Web.Areas.Admin.Models.Orders;
//using Specter.Web.Areas.Admin.Models.Payments;
//using Specter.Web.Areas.Admin.Models.Plugins;
//using Specter.Web.Areas.Admin.Models.Polls;
using Codestetic.Web.Areas.Admin.Models.Settings;
//using Specter.Web.Areas.Admin.Models.Topics;
//using Specter.Web.Core.Domain.Blogs;
//using Specter.Web.Core.Domain.Catalog;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Cms;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Directory;
//using Specter.Web.Core.Domain.Discounts;
//using Specter.Web.Core.Domain.Forums;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Core.Domain.Logging;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Core.Domain.Messages;
//using Specter.Web.Core.Domain.News;
//using Specter.Web.Core.Domain.Orders;
//using Specter.Web.Core.Domain.Polls;
//using Specter.Web.Core.Domain.Topics;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Core.Plugins;
//using Specter.Web.Services.Authentication.External;
using Codestetic.Web.Services.Cms;
//using Specter.Web.Services.Payments;
//using Specter.Web.Services.Seo;

namespace Codestetic.Web.Areas.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            // special mapper, that avoids DbUpdate exceptions in cases where
            // optional (nullable) int FK properties are 0 instead of null 
            // after mapping model > entity.
            Mapper.CreateMap<int, int?>().ConvertUsing((src) => src == 0 ? (int?)null : src);

            #region Address
            Mapper.CreateMap<Address, AddressModel>()
                .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                .ForMember(dest => dest.CompanyEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CompanyRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                .ForMember(dest => dest.CountryName, mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null));
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Country, mo => mo.Ignore());
            #endregion Address
            #region Countries
            Mapper.CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion
            #region Language
            Mapper.CreateMap<Language, LanguageModel>()
                .ForMember(dest => dest.FlagFileNames, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LanguageModel, Language>()
                .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore());
            #endregion Language
            #region Email account
            Mapper.CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<EmailAccountModel, EmailAccount>();
            #endregion Email account
            #region Message template
            Mapper.CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(dest => dest.TokensTree, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<MessageTemplateModel, MessageTemplate>();
            #endregion Message template
            #region Queued email
            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName, mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                .ForMember(dest => dest.ReplyTo, mo => mo.Ignore())
                .ForMember(dest => dest.ReplyToName, mo => mo.Ignore());
            #endregion Queued email
            #region Campaign
            //Mapper.CreateMap<Campaign, CampaignModel>()
            //    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
            //    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
            //    .ForMember(dest => dest.TestEmail, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<CampaignModel, Campaign>()
            //    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
            #endregion Campaign
            #region Topcis
            //Mapper.CreateMap<Topic, TopicModel>()
            //    .ForMember(dest => dest.Url, mo => mo.Ignore())
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableWidgetZones, mo => mo.Ignore());
            //Mapper.CreateMap<TopicModel, Topic>();
            #endregion Topcis
            #region Category
            //Mapper.CreateMap<Category, CategoryModel>()
            //    .ForMember(dest => dest.AvailableCategoryTemplates, mo => mo.Ignore())
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.Breadcrumb, mo => mo.Ignore())
            //    .ForMember(dest => dest.ParentCategoryBreadcrumb, mo => mo.Ignore()) // codehint: sm-edit
            //    .ForMember(dest => dest.AvailableDiscounts, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedDiscountIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
            //    .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
            //    .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore());
            //Mapper.CreateMap<CategoryModel, Category>()
            //    .ForMember(dest => dest.HasDiscountsApplied, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.Deleted, mo => mo.Ignore())
            //    .ForMember(dest => dest.AppliedDiscounts, mo => mo.Ignore())
            //    .ForMember(dest => dest.Picture, mo => mo.Ignore());
            #endregion Category
            #region Logs
            Mapper.CreateMap<Log, LogModel>()
                .ForMember(dest => dest.UserEmail, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                .ForMember(dest => dest.LogLevelHint, mo => mo.Ignore());
            Mapper.CreateMap<LogModel, Log>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                .ForMember(dest => dest.User, mo => mo.Ignore());
            #endregion Logs
            #region ActivityLogType
            Mapper.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLogType, ActivityLogTypeModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                .ForMember(dest => dest.UserEmail, mo => mo.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion ActivityLogType
            #region Currencies
            Mapper.CreateMap<Currency, CurrencyModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.IsPrimaryExchangeRateCurrency, mo => mo.Ignore())
                .ForMember(dest => dest.IsPrimaryStoreCurrency, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableDomainEndings, mo => mo.Ignore());
            Mapper.CreateMap<CurrencyModel, Currency>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion Currencies
            #region ContentSlider slides
            Mapper.CreateMap<ContentSliderSettings, ContentSliderSettingsModel>()
                .ForMember(dest => dest.Id, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContentSliderSettingsModel, ContentSliderSettings>();

            Mapper.CreateMap<ContentSliderSlideSettings, ContentSliderSlideModel>()
                .ForMember(dest => dest.Id, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                .ForMember(dest => dest.SlideIndex, mo => mo.Ignore());
            Mapper.CreateMap<ContentSliderSlideModel, ContentSliderSlideSettings>();

            Mapper.CreateMap<ContentSliderButtonSettings, ContentSliderButtonModel>()
                .ForMember(dest => dest.Id, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContentSliderButtonModel, ContentSliderButtonSettings>();
            #endregion ContentSlider slides
            #region Attribute combinations
            //Mapper.CreateMap<ProductVariantAttributeCombination, ProductVariantAttributeCombinationModel>()
            //    .ForMember(dest => dest.AssignablePictures, mo => mo.Ignore())
            //    .ForMember(dest => dest.ProductVariantAttributes, mo => mo.Ignore())
            //    .ForMember(dest => dest.AssignedPictureIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableDeliveryTimes, mo => mo.Ignore())
            //    .ForMember(dest => dest.ProductUrl, mo => mo.Ignore())
            //    .ForMember(dest => dest.ProductUrlTitle, mo => mo.Ignore())
            //    .ForMember(dest => dest.Warnings, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
            //    .AfterMap((src, dest) => dest.AssignedPictureIds = src.GetAssignedPictureIds());
            //Mapper.CreateMap<ProductVariantAttributeCombinationModel, ProductVariantAttributeCombination>()
            //    .ForMember(dest => dest.DeliveryTime, mo => mo.Ignore())
            //    .ForMember(dest => dest.Product, mo => mo.Ignore())
            //    .ForMember(dest => dest.AssignedPictureIds, mo => mo.Ignore())
            //    .AfterMap((src, dest) => dest.SetAssignedPictureIds(src.AssignedPictureIds));
            #endregion Attribute combinations
            #region Payment methods
            //Mapper.CreateMap<IPaymentMethod, PaymentMethodModel>()
            //    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
            //    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
            //    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
            //    //.ForMember(dest => dest.RecurringPaymentType, mo => mo.Ignore())
            //    .ForMember(dest => dest.Active, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion Payment methods
            #region External authentication methods
            //Mapper.CreateMap<IExternalAuthenticationMethod, AuthenticationMethodModel>()
            //    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
            //    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
            //    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
            //    .ForMember(dest => dest.Active, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion External authentication methods
            #region Widgets
            //Mapper.CreateMap<IWidgetPlugin, WidgetModel>()
            //    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
            //    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
            //    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
            //    .ForMember(dest => dest.Active, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion Widgets
            #region Plugins
            //Mapper.CreateMap<PluginDescriptor, PluginModel>()
            //    .ForMember(dest => dest.ConfigurationUrl, mo => mo.Ignore())
            //    .ForMember(dest => dest.CanChangeEnabled, mo => mo.Ignore())
            //    .ForMember(dest => dest.IsEnabled, mo => mo.Ignore())
            //    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.MarkerUrl, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion Plugins
            #region NewsLetter subscriptions
            Mapper.CreateMap<NewsLetterSubscription, NewsLetterSubscriptionModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<NewsLetterSubscriptionModel, NewsLetterSubscription>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.NewsLetterSubscriptionGuid, mo => mo.Ignore());
            #endregion NewsLetter subscriptions
            #region Forums
            //Mapper.CreateMap<ForumGroup, ForumGroupModel>()
            //    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
            //    .ForMember(dest => dest.ForumModels, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<ForumGroupModel, ForumGroup>()
            //    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.Forums, mo => mo.Ignore());
            //Mapper.CreateMap<Forum, ForumModel>()
            //    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
            //    .ForMember(dest => dest.ForumGroups, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<ForumModel, Forum>()
            //    .ForMember(dest => dest.NumTopics, mo => mo.Ignore())
            //    .ForMember(dest => dest.NumPosts, mo => mo.Ignore())
            //    .ForMember(dest => dest.LastTopicId, mo => mo.Ignore())
            //    .ForMember(dest => dest.LastPostId, mo => mo.Ignore())
            //    .ForMember(dest => dest.LastPostUserId, mo => mo.Ignore())
            //    .ForMember(dest => dest.LastPostTime, mo => mo.Ignore())
            //    .ForMember(dest => dest.ForumGroup, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion Forums
            #region Blogs
            //Mapper.CreateMap<BlogPost, BlogPostModel>()
            //    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(src.LanguageId, true, false)))
            //    .ForMember(dest => dest.Comments, mo => mo.Ignore())
            //    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
            //    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<BlogPostModel, BlogPost>()
            //    .ForMember(dest => dest.BlogComments, mo => mo.Ignore())
            //    .ForMember(dest => dest.Language, mo => mo.Ignore())
            //    .ForMember(dest => dest.ApprovedCommentCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.NotApprovedCommentCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
            #endregion Blogs
            #region News
            //Mapper.CreateMap<NewsItem, NewsItemModel>()
            //    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(src.LanguageId, true, false)))
            //    .ForMember(dest => dest.Comments, mo => mo.Ignore())
            //    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
            //    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<NewsItemModel, NewsItem>()
            //    .ForMember(dest => dest.NewsComments, mo => mo.Ignore())
            //    .ForMember(dest => dest.Language, mo => mo.Ignore())
            //    .ForMember(dest => dest.ApprovedCommentCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.NotApprovedCommentCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
            #endregion News
            #region Polls
            //Mapper.CreateMap<Poll, PollModel>()
            //    .ForMember(dest => dest.StartDate, mo => mo.Ignore())
            //    .ForMember(dest => dest.EndDate, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<PollModel, Poll>()
            //    .ForMember(dest => dest.PollAnswers, mo => mo.Ignore())
            //    .ForMember(dest => dest.Language, mo => mo.Ignore())
            //    .ForMember(dest => dest.StartDateUtc, mo => mo.Ignore())
            //    .ForMember(dest => dest.EndDateUtc, mo => mo.Ignore());
            #endregion Polls
            #region User roles
            Mapper.CreateMap<UserRole, UserRoleModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<UserRoleModel, UserRole>()
                .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());
            #endregion User roles
            #region Product attributes
            //Mapper.CreateMap<ProductAttribute, ProductAttributeModel>()
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<ProductAttributeModel, ProductAttribute>();
            #endregion Product attributes
            #region Specification attributes
            //Mapper.CreateMap<SpecificationAttribute, SpecificationAttributeModel>()
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<SpecificationAttributeModel, SpecificationAttribute>()
            //    .ForMember(dest => dest.SpecificationAttributeOptions, mo => mo.Ignore());
            //Mapper.CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionModel>()
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.Multiple, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<SpecificationAttributeOptionModel, SpecificationAttributeOption>()
            //    .ForMember(dest => dest.SpecificationAttribute, mo => mo.Ignore())
            //    .ForMember(dest => dest.ProductSpecificationAttributes, mo => mo.Ignore());
            #endregion Specification attributes
            #region Checkout attributes
            //Mapper.CreateMap<CheckoutAttribute, CheckoutAttributeModel>()
            //    .ForMember(dest => dest.AvailableTaxCategories, mo => mo.Ignore())
            //    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<CheckoutAttributeModel, CheckoutAttribute>()
            //    .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
            //    .ForMember(dest => dest.CheckoutAttributeValues, mo => mo.Ignore());
            //Mapper.CreateMap<CheckoutAttributeValue, CheckoutAttributeValueModel>()
            //    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
            //    .ForMember(dest => dest.BaseWeightIn, mo => mo.Ignore())
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<CheckoutAttributeValueModel, CheckoutAttributeValue>()
            //    .ForMember(dest => dest.CheckoutAttribute, mo => mo.Ignore());
            #endregion Checkout attributes
            #region Discounts
            //Mapper.CreateMap<Discount, DiscountModel>()
            //    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
            //    .ForMember(dest => dest.AddDiscountRequirement, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableDiscountRequirementRules, mo => mo.Ignore())
            //    .ForMember(dest => dest.DiscountRequirementMetaInfos, mo => mo.Ignore())
            //    .ForMember(dest => dest.AppliedToCategoryModels, mo => mo.Ignore())
            //    .ForMember(dest => dest.AppliedToProductModels, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<DiscountModel, Discount>()
            //    .ForMember(dest => dest.DiscountType, mo => mo.Ignore())
            //    .ForMember(dest => dest.DiscountLimitation, mo => mo.Ignore())
            //    .ForMember(dest => dest.DiscountRequirements, mo => mo.Ignore())
            //    .ForMember(dest => dest.AppliedToCategories, mo => mo.Ignore())
            //    .ForMember(dest => dest.AppliedToProducts, mo => mo.Ignore());
            #endregion Discounts
            #region Settings
            //Mapper.CreateMap<NewsSettings, NewsSettingsModel>();
            //Mapper.CreateMap<NewsSettingsModel, NewsSettings>();
            //Mapper.CreateMap<ForumSettings, ForumSettingsModel>()
            //    .ForMember(dest => dest.ForumEditorValues, mo => mo.Ignore());
            //Mapper.CreateMap<ForumSettingsModel, ForumSettings>()
            //    .ForMember(dest => dest.TopicSubjectMaxLength, mo => mo.Ignore())
            //    .ForMember(dest => dest.StrippedTopicMaxLength, mo => mo.Ignore())
            //    .ForMember(dest => dest.PostMaxLength, mo => mo.Ignore())
            //    .ForMember(dest => dest.TopicPostsPageLinkDisplayCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.LatestUserPostsPageSize, mo => mo.Ignore())
            //    .ForMember(dest => dest.PrivateMessagesPageSize, mo => mo.Ignore())
            //    .ForMember(dest => dest.ForumSubscriptionsPageSize, mo => mo.Ignore())
            //    .ForMember(dest => dest.PMSubjectMaxLength, mo => mo.Ignore())
            //    .ForMember(dest => dest.PMTextMaxLength, mo => mo.Ignore())
            //    .ForMember(dest => dest.HomePageActiveDiscussionsTopicCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.ActiveDiscussionsPageTopicCount, mo => mo.Ignore())
            //    .ForMember(dest => dest.ForumSearchTermMinimumLength, mo => mo.Ignore());
            //Mapper.CreateMap<BlogSettings, BlogSettingsModel>()
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<BlogSettingsModel, BlogSettings>();
            //Mapper.CreateMap<ShippingSettings, ShippingSettingsModel>()
            //    .ForMember(dest => dest.ShippingOriginAddress, mo => mo.Ignore());
            //Mapper.CreateMap<ShippingSettingsModel, ShippingSettings>()
            //    .ForMember(dest => dest.ActiveShippingRateComputationMethodSystemNames, mo => mo.Ignore())
            //    .ForMember(dest => dest.ReturnValidOptionsIfThereAreAny, mo => mo.Ignore());
            //Mapper.CreateMap<CatalogSettings, CatalogSettingsModel>()
            //    .ForMember(dest => dest.AvailableDefaultViewModes, mo => mo.Ignore());
            //Mapper.CreateMap<CatalogSettingsModel, CatalogSettings>()
            //    .ForMember(dest => dest.PageShareCode, mo => mo.Ignore())
            //    .ForMember(dest => dest.DefaultProductRatingValue, mo => mo.Ignore())
            //    .ForMember(dest => dest.ProductSearchTermMinimumLength, mo => mo.Ignore())
            //    .ForMember(dest => dest.UseSmallProductBoxOnHomePage, mo => mo.Ignore())
            //    .ForMember(dest => dest.IncludeFeaturedProductsInNormalLists, mo => mo.Ignore())
            //    //.ForMember(dest => dest.DefaultPageSizeOptions, mo => mo.Ignore())
            //    .ForMember(dest => dest.DefaultCategoryPageSizeOptions, mo => mo.Ignore()) // codehint: Obsolete soon
            //    .ForMember(dest => dest.DefaultManufacturerPageSizeOptions, mo => mo.Ignore()) // codehint: Obsolete soon
            //    .ForMember(dest => dest.MaximumBackInStockSubscriptions, mo => mo.Ignore())
            //    .ForMember(dest => dest.DisplayTierPricesWithDiscounts, mo => mo.Ignore())
            //    .ForMember(dest => dest.FileUploadMaximumSizeBytes, mo => mo.Ignore())
            //    .ForMember(dest => dest.FileUploadAllowedExtensions, mo => mo.Ignore())
            //    .ForMember(dest => dest.ShowProductImagesInSearchAutoComplete, mo => mo.Ignore())
            //    .ForMember(dest => dest.ProductSearchPageSize, mo => mo.Ignore())
            //    .ForMember(dest => dest.ManufacturersBlockItemsToDisplay, mo => mo.Ignore());
            //Mapper.CreateMap<RewardPointsSettings, RewardPointsSettingsModel>()
            //    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
            //    .ForMember(dest => dest.PointsForPurchases_OverrideForStore, mo => mo.Ignore());
            //Mapper.CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();
            //Mapper.CreateMap<OrderSettings, OrderSettingsModel>()
            //    .ForMember(dest => dest.ReturnRequestReasonsParsed, mo => mo.Ignore())
            //    .ForMember(dest => dest.ReturnRequestActionsParsed, mo => mo.Ignore())
            //    .ForMember(dest => dest.GiftCards_Activated_OrderStatuses, mo => mo.Ignore())
            //    .ForMember(dest => dest.GiftCards_Deactivated_OrderStatuses, mo => mo.Ignore())
            //    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
            //    .ForMember(dest => dest.OrderIdent, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<OrderSettingsModel, OrderSettings>()
            //    .ForMember(dest => dest.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, mo => mo.Ignore())
            //    .ForMember(dest => dest.ReturnRequestReasons, mo => mo.Ignore())
            //    .ForMember(dest => dest.ReturnRequestActions, mo => mo.Ignore())
            //    .ForMember(dest => dest.MinimumOrderPlacementInterval, mo => mo.Ignore());
            //Mapper.CreateMap<ShoppingCartSettings, ShoppingCartSettingsModel>();
            //Mapper.CreateMap<ShoppingCartSettingsModel, ShoppingCartSettings>()
            //    .ForMember(dest => dest.RoundPricesDuringCalculation, mo => mo.Ignore())
            //    .ForMember(dest => dest.MoveItemsFromWishlistToCart, mo => mo.Ignore());
            Mapper.CreateMap<MediaSettings, MediaSettingsModel>()
                .ForMember(dest => dest.PicturesStoredIntoDatabase, mo => mo.Ignore())
                .ForMember(dest => dest.AvailablePictureZoomTypes, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<MediaSettingsModel, MediaSettings>()
                //.ForMember(dest => dest.DefaultPictureZoomEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.DefaultImageQuality, mo => mo.Ignore())
                .ForMember(dest => dest.MultipleThumbDirectories, mo => mo.Ignore())
                .ForMember(dest => dest.AutoCompleteSearchThumbPictureSize, mo => mo.Ignore());
            Mapper.CreateMap<UserSettings, AdvanceUserSettingsModel.UserSettingsModel>();
            Mapper.CreateMap<AdvanceUserSettingsModel.UserSettingsModel, UserSettings>()
                .ForMember(dest => dest.HashedPasswordFormat, mo => mo.Ignore())
                .ForMember(dest => dest.PasswordMinLength, mo => mo.Ignore())
                .ForMember(dest => dest.AvatarMaximumSizeBytes, mo => mo.Ignore())
                .ForMember(dest => dest.DownloadableProductsValidateUser, mo => mo.Ignore())
                .ForMember(dest => dest.OnlineUserMinutes, mo => mo.Ignore())
                .ForMember(dest => dest.PrefillLoginUsername, mo => mo.Ignore())
                .ForMember(dest => dest.PrefillLoginPwd, mo => mo.Ignore());
            Mapper.CreateMap<AddressSettings, AdvanceUserSettingsModel.AddressSettingsModel>();
            Mapper.CreateMap<AdvanceUserSettingsModel.AddressSettingsModel, AddressSettings>();
            #endregion Settings
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
