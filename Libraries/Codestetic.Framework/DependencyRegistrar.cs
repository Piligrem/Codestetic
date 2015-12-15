using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Module = Autofac.Module;

using Codestetic.Web.Core.Infrastructure.DependencyManagement;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Data;
using Codestetic.Web.Core.Data.Hooks;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Framework.Users;
using Codestetic.Web.Core.Fakes;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Core.Configuration;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Services.Events;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Core;
using Codestetic.Web.Services.Authentication;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Services;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Framework.Localization;
using Codestetic.Web.Core.Localization;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Framework.UI;
using Codestetic.Web.Services.Cms;
using Codestetic.Web.Services.Media;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Framework.Mvc.Routes;
using Codestetic.Web.Framework.EmbeddedViews;
using Codestetic.Web.Framework.Mvc.Bundles;
using Codestetic.Web.Services.Tasks;
using Codestetic.Web.Services.Messages;
using Codestetic.Web.Core.Email;
using Codestetic.Web.Services.Directory;
//using Specter.Web.Framework.WebApi.Configuration;
//using Specter.Web.Framework.WebApi;
using Codestetic.Web.Core.Plugins;
using Codestetic.Web.Services.GeoZones;
using Codestetic.Web.Services.Tracker;
using Codestetic.Web.Core.Packaging;
using Codestetic.Web.Services.Devices;
using Codestetic.Web.Services.Seo;
using Codestetic.Web.Services.Forums;
using Codestetic.Web.Services.Topics;
using Codestetic.Web.Services.Authentication.External;
using Codestetic.Web.Services.Map;

namespace Codestetic.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // modules
            builder.RegisterModule(new AutofacWebTypesModule());
            //builder.RegisterModule(new IdentityModule(typeFinder));
            builder.RegisterModule(new DbModule(typeFinder));
            builder.RegisterModule(new CachingModule());
            builder.RegisterModule(new LocalizationModule());
            builder.RegisterModule(new LoggingModule());
            builder.RegisterModule(new EventModule(typeFinder));
            builder.RegisterModule(new MessagingModule());
            builder.RegisterModule(new WebModule(typeFinder));
            //builder.RegisterModule(new WebApiModule(typeFinder));
            builder.RegisterModule(new UiModule(typeFinder));
            //builder.RegisterModule(new IOModule());
            builder.RegisterModule(new PackagingModule());

            // sources
            builder.RegisterSource(new SettingsSource());

            // web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerRequest();

            // plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().SingleInstance(); // xxx (http)

            // work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().WithStaticCache().InstancePerRequest();

            // site context
            builder.RegisterType<WebSiteContext>().As<ISiteContext>().InstancePerRequest();

            #region Services
            //builder.RegisterType<BackInStockSubscriptionService>().As<IBackInStockSubscriptionService>().InstancePerHttpRequest();
            //builder.RegisterType<CategoryService>().As<ICategoryService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<CompareProductsService>().As<ICompareProductsService>().InstancePerHttpRequest();
            //builder.RegisterType<RecentlyViewedProductsService>().As<IRecentlyViewedProductsService>().InstancePerHttpRequest();
            //builder.RegisterType<ManufacturerService>().As<IManufacturerService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<PriceCalculationService>().As<IPriceCalculationService>().InstancePerHttpRequest();
            //builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerHttpRequest();
            //builder.RegisterType<ProductAttributeFormatter>().As<IProductAttributeFormatter>().InstancePerLifetimeScope();
            //builder.RegisterType<ProductAttributeParser>().As<IProductAttributeParser>().InstancePerHttpRequest();
            //builder.RegisterType<ProductAttributeService>().As<IProductAttributeService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<ProductService>().As<IProductService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<CopyProductService>().As<ICopyProductService>().InstancePerHttpRequest();
            //builder.RegisterType<SpecificationAttributeService>().As<ISpecificationAttributeService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<ProductTemplateService>().As<IProductTemplateService>().InstancePerHttpRequest();
            //builder.RegisterType<CategoryTemplateService>().As<ICategoryTemplateService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<ManufacturerTemplateService>().As<IManufacturerTemplateService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<ProductTagService>().As<IProductTagService>().WithStaticCache().InstancePerHttpRequest();

            //builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerHttpRequest();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerRequest();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().WithRequestCache().InstancePerRequest();
            //builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerHttpRequest();
            builder.RegisterType<MaintenanceService>().As<IMaintenanceService>().InstancePerRequest();

            //builder.RegisterType<UserContentService>().As<IUserContentService>().WithRequestCache().InstancePerHttpRequest();
            builder.RegisterType<UserService>().As<IUserService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>().InstancePerRequest();
            builder.RegisterType<UserReportService>().As<IUserReportService>().InstancePerRequest();

            builder.RegisterType<UserService>().As<IUserService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<DeviceService>().As<IDeviceService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<DeviceReportService>().As<IDeviceReportService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<SessionService>().As<ISessionService>().WithRequestCache().InstancePerRequest();

            builder.RegisterType<PermissionService>().As<IPermissionService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<AclService>().As<IAclService>().WithStaticCache().InstancePerRequest();

            //builder.RegisterType<GeoCountryLookup>().As<IGeoCountryLookup>().InstancePerHttpRequest();
            builder.RegisterType<CountryService>().As<ICountryService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<CurrencyService>().As<ICurrencyService>().WithRequestCache().InstancePerRequest();

            //builder.RegisterType<DeliveryTimeService>().As<IDeliveryTimeService>().WithRequestCache().InstancePerHttpRequest();

            //builder.RegisterType<MeasureService>().As<IMeasureService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().WithRequestCache().InstancePerHttpRequest();

            //builder.RegisterType<SiteService>().As<ISiteService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<SiteMappingService>().As<ISiteMappingService>().WithStaticCache().InstancePerHttpRequest();

            //builder.RegisterType<DiscountService>().As<IDiscountService>().WithRequestCache().InstancePerHttpRequest();

            builder.RegisterType<SettingService>().As<ISettingService>().WithStaticCache().InstancePerRequest();

            builder.RegisterType<DownloadService>().As<IDownloadService>().InstancePerRequest();
            builder.RegisterType<ImageCache>().As<IImageCache>().InstancePerRequest();
            builder.RegisterType<ImageResizerService>().As<IImageResizerService>().SingleInstance();
            builder.RegisterType<PictureService>().As<IPictureService>().InstancePerRequest();

            //builder.RegisterType<CheckoutAttributeFormatter>().As<ICheckoutAttributeFormatter>().InstancePerHttpRequest();
            //builder.RegisterType<CheckoutAttributeParser>().As<ICheckoutAttributeParser>().InstancePerHttpRequest();
            //builder.RegisterType<CheckoutAttributeService>().As<ICheckoutAttributeService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<GiftCardService>().As<IGiftCardService>().InstancePerHttpRequest();
            //builder.RegisterType<OrderService>().As<IOrderService>().InstancePerHttpRequest();
            //builder.RegisterType<OrderReportService>().As<IOrderReportService>().InstancePerHttpRequest();
            //builder.RegisterType<OrderProcessingService>().As<IOrderProcessingService>().InstancePerHttpRequest();
            //builder.RegisterType<OrderTotalCalculationService>().As<IOrderTotalCalculationService>().InstancePerHttpRequest();
            //builder.RegisterType<ShoppingCartService>().As<IShoppingCartService>().InstancePerHttpRequest();

            //builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerHttpRequest();

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerRequest();
            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerRequest();

            builder.RegisterType<UrlRecordService>().As<IUrlRecordService>().WithStaticCache().InstancePerRequest();

            //builder.RegisterType<ShipmentService>().As<IShipmentService>().InstancePerHttpRequest();
            //builder.RegisterType<ShippingService>().As<IShippingService>().WithRequestCache().InstancePerHttpRequest();

            //builder.RegisterType<TaxCategoryService>().As<ITaxCategoryService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<TaxService>().As<ITaxService>().InstancePerHttpRequest();

            builder.RegisterType<ForumService>().As<IForumService>().WithRequestCache().InstancePerRequest();

            //builder.RegisterType<PollService>().As<IPollService>().WithRequestCache().InstancePerHttpRequest();
            //builder.RegisterType<BlogService>().As<IBlogService>().WithRequestCache().InstancePerHttpRequest();
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerRequest();
            builder.RegisterType<TopicService>().As<ITopicService>().InstancePerRequest();
            //builder.RegisterType<NewsService>().As<INewsService>().WithRequestCache().InstancePerHttpRequest();

            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerRequest();
            //builder.RegisterType<SitemapGenerator>().As<ISitemapGenerator>().InstancePerHttpRequest();
            builder.RegisterType<PageAssetsBuilder>().As<IPageAssetsBuilder>().InstancePerRequest();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerRequest();

            //builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerHttpRequest();
            //builder.RegisterType<ImportManager>().As<IImportManager>().InstancePerHttpRequest();
            builder.RegisterType<MobileDeviceHelper>().As<IMobileDeviceHelper>().InstancePerRequest();
            //builder.RegisterType<PdfService>().As<IPdfService>().InstancePerHttpRequest();

            builder.RegisterType<ExternalAuthorizer>().As<IExternalAuthorizer>().InstancePerRequest();
            builder.RegisterType<OpenAuthenticationService>().As<IOpenAuthenticationService>().InstancePerRequest();

            //builder.RegisterType<FilterService>().As<IFilterService>().InstancePerHttpRequest();
            builder.RegisterType<CommonServices>().As<ICommonServices>().WithStaticCache().InstancePerRequest();

            //builder.RegisterType<PerfLoggerService>().As<IPerfLoggerService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<PanelService>().As<IPanelService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<MapLayerService>().As<IMapLayerService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<DevicePositionService>().As<IDevicePositionService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<DeviceIndicatorService>().As<IDeviceIndicatorService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<GeoZoneService>().As<IGeoZoneService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<NotifyService>().As<INotifyService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<TrackService>().As<ITrackService>().WithRequestCache().InstancePerRequest();
            #endregion Services
        }

        public int Order
        {
            get { return -100; }
        }
    }

    #region Modules
    public class DbModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public DbModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => DataSettings.Current).As<DataSettings>().InstancePerDependency();
            builder.Register(x => new EfDataProviderFactory(x.Resolve<DataSettings>())).As<DataProviderFactory>().InstancePerDependency();

            builder.Register(x => x.Resolve<DataProviderFactory>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<DataProviderFactory>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();

            if (DataSettings.Current.IsValid())
            {
                // register DB Hooks (only when app was installed properly)

                Func<Type, Type> findHookedType = (t) =>
                {
                    var x = t;
                    while (x != null)
                    {
                        if (x.IsGenericType)
                        {
                            return x.GetGenericArguments()[0];
                        }
                        x = x.BaseType;
                    }

                    return typeof(object);
                };

                var hooks = _typeFinder.FindClassesOfType(typeof(IHook));
                foreach (var hook in hooks)
                {
                    var hookedType = findHookedType(hook);

                    var registration = builder.RegisterType(hook)
                        .As(typeof(IPreActionHook).IsAssignableFrom(hook) ? typeof(IPreActionHook) : typeof(IPostActionHook))
                        .InstancePerRequest();

                    registration.WithMetadata<HookMetadata>(m =>
                    {
                        m.For(em => em.HookedType, hookedType);
                    });
                }

                builder.Register<IDbContext>(c => new SpecterObjectContext(DataSettings.Current.DataConnectionString))
                    .PropertiesAutowired(PropertyWiringOptions.None)
                    .InstancePerRequest();
            }
            else
            {
                builder.Register<IDbContext>(c => new SpecterObjectContext(DataSettings.Current.DataConnectionString)).InstancePerRequest();
            }

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();

            builder.Register<DbQuerySettings>(c =>
            {
                var aclService = c.Resolve<IAclService>();
                return new DbQuerySettings(!aclService.HasActiveAcl);
            }).InstancePerRequest();
        }
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var querySettingsProperty = FindQuerySettingsProperty(registration.Activator.LimitType);

            if (querySettingsProperty == null)
                return;

            registration.Activated += (sender, e) =>
            {
                var querySettings = e.Context.Resolve<DbQuerySettings>();
                querySettingsProperty.SetValue(e.Instance, querySettings, null);
            };
        }
        private static PropertyInfo FindQuerySettingsProperty(Type type)
        {
            return type.GetProperty("QuerySettings", typeof(DbQuerySettings));
        }
    }
    public class CachingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StaticCache>().As<ICache>().Keyed<ICache>(typeof(StaticCache)).SingleInstance();
            builder.RegisterType<AspNetCache>().As<ICache>().Keyed<ICache>(typeof(AspNetCache)).InstancePerRequest();
            builder.RegisterType<RequestCache>().As<ICache>().Keyed<ICache>(typeof(RequestCache)).InstancePerRequest();

            builder.RegisterType<CacheManager<StaticCache>>()
                .As<ICacheManager>()
                .Named<ICacheManager>("static")
                .SingleInstance();
            builder.RegisterType<CacheManager<AspNetCache>>()
                .As<ICacheManager>()
                .Named<ICacheManager>("aspnet")
                .InstancePerRequest();
            builder.RegisterType<CacheManager<RequestCache>>()
                .As<ICacheManager>()
                .Named<ICacheManager>("request")
                .InstancePerRequest();

            // Register resolving delegate
            builder.Register<Func<Type, ICache>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return keyed => cc.ResolveKeyed<ICache>(keyed);
            });

            builder.Register<Func<string, ICacheManager>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<ICacheManager>(named);
            });

            builder.Register<Func<string, Lazy<ICacheManager>>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<Lazy<ICacheManager>>(named);
            });
        }
    }
    public class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LanguageService>().As<ILanguageService>().WithRequestCache().InstancePerRequest();

            //builder.RegisterType<TelerikLocalizationServiceFactory>().As<Telerik.Web.Mvc.Infrastructure.ILocalizationServiceFactory>().InstancePerHttpRequest();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithStaticCache() // pass StaticCache as ICache (cache settings between requests)
                .InstancePerRequest();

            builder.RegisterType<Text>().As<IText>().InstancePerRequest();

            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithStaticCache() // pass StaticCache as ICache (cache settings between requests)
                .InstancePerRequest();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var userProperty = FindUserProperty(registration.Activator.LimitType);

            if (userProperty == null)
                return;

            registration.Activated += (sender, e) =>
            {
                Localizer localizer = e.Context.Resolve<IText>().Get;
                userProperty.SetValue(e.Instance, localizer, null);
            };
        }

        private static PropertyInfo FindUserProperty(Type type)
        {
            return type.GetProperty("T", typeof(Localizer));
        }
    }
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Notifier>().As<INotifier>().InstancePerRequest();
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerRequest();
            builder.RegisterType<UserActivityService>().As<IUserActivityService>().WithRequestCache().InstancePerRequest();
            //builder.RegisterType<PerfLoggerService>().As<IPerfLoggerService>().InstancePerRequest();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (!DataSettings.DatabaseIsInstalled())
                return;

            var implementationType = registration.Activator.LimitType;

            // build an array of actions on this type to assign loggers to member properties
            var injectors = BuildLoggerInjectors(implementationType).ToArray();

            // if there are no logger properties, there's no reason to hook the activated event
            if (!injectors.Any())
                return;

            // otherwise, when an instance of this component is activated, inject the loggers on the instance
            registration.Activated += (s, e) =>
            {
                foreach (var injector in injectors)
                    injector(e.Context, e.Instance);
            };
        }

        private IEnumerable<Action<IComponentContext, object>> BuildLoggerInjectors(Type componentType)
        {
            if (DataSettings.DatabaseIsInstalled())
            {
                // Look for settable properties of type "ILogger" 
                var loggerProperties = componentType
                    .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new
                    {
                        PropertyInfo = p,
                        p.PropertyType,
                        IndexParameters = p.GetIndexParameters(),
                        Accessors = p.GetAccessors(false)
                    })
                    .Where(x => x.PropertyType == typeof(ILogger)) // must be a logger
                    .Where(x => x.IndexParameters.Count() == 0) // must not be an indexer
                    .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void)); //must have get/set, or only set

                // Return an array of actions that resolve a logger and assign the property
                foreach (var entry in loggerProperties)
                {
                    var propertyInfo = entry.PropertyInfo;

                    yield return (ctx, instance) =>
                    {
                        string component = componentType.ToString();
                        var logger = ctx.Resolve<ILogger>();
                        propertyInfo.SetValue(instance, logger, null);
                    };
                }
            }
        }
    }
    public class EventModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public EventModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterGeneric(typeof(DefaultConsumerFactory<>)).As(typeof(IConsumerFactory<>)).InstancePerDependency();

            // Register event consumers
            var consumerTypes = _typeFinder.FindClassesOfType(typeof(IConsumer<>));
            foreach (var consumerType in consumerTypes)
            {
                Type[] implementedInterfaces = consumerType.FindInterfaces(IsConsumerInterface, typeof(IConsumer<>));

                var registration = builder.RegisterType(consumerType).As(implementedInterfaces);

                var isActive = PluginManager.IsActivePluginAssembly(consumerType.Assembly);
                var shouldExecuteAsync = consumerType.GetAttribute<AsyncConsumerAttribute>(false) != null;

                registration.WithMetadata<EventConsumerMetadata>(m =>
                {
                    m.For(em => em.IsActive, isActive);
                    m.For(em => em.ExecuteAsync, shouldExecuteAsync);
                });

                if (!shouldExecuteAsync)
                    registration.InstancePerRequest();

            }
        }

        private static bool IsConsumerInterface(Type type, object criteria)
        {
            var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
            return isMatch;
        }
    }
    public class WebModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public WebModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var foundAssemblies = _typeFinder.GetAssemblies().ToArray();

            builder.RegisterModule(new AutofacWebTypesModule());
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerRequest();

            // register all controllers
            builder.RegisterControllers(foundAssemblies);

            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
            builder.RegisterType<BundlePublisher>().As<IBundlePublisher>().SingleInstance();
            builder.RegisterType<BundleBuilder>().As<IBundleBuilder>().InstancePerRequest();
        }
    }
    //public class WebApiModule : Module
    //{
    //    private readonly ITypeFinder _typeFinder;

    //    public WebApiModule(ITypeFinder typeFinder)
    //    {
    //        _typeFinder = typeFinder;
    //    }

    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        var foundAssemblies = _typeFinder.GetAssemblies().ToArray();

    //        // register all api controllers
    //        builder.RegisterApiControllers(foundAssemblies);

    //        builder.RegisterType<WebApiConfigurationPublisher>().As<IWebApiConfigurationPublisher>().SingleInstance();
    //    }

    //    protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
    //    {
    //        var baseType = typeof(WebApiEntityController<,>);
    //        var type = registration.Activator.LimitType;
    //        Type implementingType;

    //        if (!type.IsSubClass(baseType, out implementingType))
    //            return;

    //        var repoProperty = FindRepositoryProperty(type, implementingType.GetGenericArguments()[0]);
    //        var serviceProperty = FindServiceProperty(type, implementingType.GetGenericArguments()[1]);

    //        if (repoProperty != null || serviceProperty != null)
    //        {
    //            registration.Activated += (sender, e) =>
    //            {
    //                if (repoProperty != null)
    //                {
    //                    var repo = e.Context.Resolve(repoProperty.PropertyType);
    //                    repoProperty.SetValue(e.Instance, repo, null);
    //                }

    //                if (serviceProperty != null)
    //                {
    //                    var service = e.Context.Resolve(serviceProperty.PropertyType);
    //                    serviceProperty.SetValue(e.Instance, service, null);
    //                }
    //            };
    //        }
    //    }

    //    private static PropertyInfo FindRepositoryProperty(Type type, Type entityType)
    //    {
    //        var pi = type.GetProperty("Repository", typeof(IRepository<>).MakeGenericType(entityType));
    //        return pi;
    //    }

    //    private static PropertyInfo FindServiceProperty(Type type, Type serviceType)
    //    {
    //        var pi = type.GetProperty("Service", serviceType);
    //        return pi;
    //    }
    //}
    public class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().WithRequestCache().InstancePerRequest();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerRequest();
            builder.RegisterType<NewsLetterSubscriptionService>().As<INewsLetterSubscriptionService>().InstancePerRequest();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerRequest();
            builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerRequest();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerRequest();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerRequest();
            builder.RegisterType<DefaultEmailSender>().As<IEmailSender>().SingleInstance(); // xxx (http)
        }
    }
    public class UiModule : Module
    {
        private readonly ITypeFinder _typeFinder;

        public UiModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // register theming services
            //builder.RegisterType<DefaultThemeRegistry>().As<IThemeRegistry>().SingleInstance();
            //builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerHttpRequest();
            //builder.RegisterType<ThemeVariablesService>().As<IThemeVariablesService>().InstancePerHttpRequest();

            // register UI component renderers
            builder.RegisterType<TabStripRenderer>().As<ComponentRenderer<TabStrip>>();
            builder.RegisterType<PagerRenderer>().As<ComponentRenderer<Pager>>();
            builder.RegisterType<WindowRenderer>().As<ComponentRenderer<Window>>();

            // Register simple (code) widgets
            var widgetTypes = _typeFinder.FindClassesOfType(typeof(IWidget)).Where(x => !typeof(IWidgetPlugin).IsAssignableFrom(x));
            foreach (var widgetType in widgetTypes)
            {
                builder.RegisterType(widgetType).As<IWidget>().Named<IWidget>(widgetType.FullName).InstancePerRequest();
            }
        }
    }
    public class PackagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PackageBuilder>().As<IPackageBuilder>().InstancePerRequest();
            builder.RegisterType<PackageInstaller>().As<IPackageInstaller>().InstancePerRequest();
            builder.RegisterType<PackageManager>().As<IPackageManager>().InstancePerRequest();
            builder.RegisterType<FolderUpdater>().As<IFolderUpdater>().InstancePerRequest();
        }
    }
    #endregion Modules

    #region Sources
    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                //var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                //yield return (IComponentRegistration)buildMethod.Invoke(null, null);

                // Performance with Fasterflect
                yield return (IComponentRegistration)Fasterflect.TryInvokeWithValuesExtensions.TryCallMethodWithValues(
                    typeof(SettingsSource),
                    null,
                    "BuildRegistration",
                    new Type[] { ts.ServiceType },
                    BindingFlags.Static | BindingFlags.NonPublic);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    return c.Resolve<ISettingService>().LoadSetting<TSettings>();
                })
                .InstancePerRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
    #endregion Sources
}
