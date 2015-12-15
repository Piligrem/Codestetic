using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Configuration;
using Codestetic.Web.Framework.Users;
using Codestetic.Web.Core.Domain.Users;
using System.Web.Security;
using Newtonsoft.Json;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Core.Domain.Security;
using Microsoft.SqlServer.Types;
using System.Data.Entity.Spatial;
using System.Data.SqlTypes;
using Codestetic.Web.Core.Domain.Logging;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Framework.Mvc.Bundles;
using System.Web.WebPages;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Framework.EmbeddedViews;
using System.Web.Hosting;
using Codestetic.Web.Services.Tasks;
using Codestetic.Web.Services.Messages;
using Codestetic.Web.Core.Domain.Messages;
using Codestetic.Web.Services.Users;
using Autofac;
using System.Data.SqlClient;
using Codestetic.Web.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNet.SignalR;
using Codestetic.Web.Areas.Admin;
using Codestetic.Web.Common;

namespace Codestetic.Web
{
    public class MvcApplication : HttpApplication
    {
        //public event Action Queried;
        //public event Action<ulong, ScaleoutMessage> Received;
        //public event Action<Exception> Faulted;

        protected void Application_Start()
        {
            //Glimpse.EF.Initialize.Start();

            var mobileDisplayMode = DisplayModeProvider.Instance.Modes.FirstOrDefault(x => x.DisplayModeId == DisplayModeProvider.MobileDisplayModeId);
            if (mobileDisplayMode != null)
                DisplayModeProvider.Instance.Modes.Remove(mobileDisplayMode);

            bool installed = DataSettings.DatabaseIsInstalled();

            if (installed)
            {
                // remove all view engines
                //ViewEngines.Engines.Clear();
                //SqlDependency.Start(DataSettings.Current.DataConnectionStringNotify);
            }

            // Initialize engine context
            EngineContext.Initialize(false);

            //// Create the container builder.
            //var builder = new ContainerBuilder();
            //builder.RegisterType<NotificationHub>().ExternallyOwned();
            //// Build the container.
            //builder.Update(EngineContext.Current.ContainerManager.Container);
            ////var container = builder.Build();
            //// Create the dependency resolver.
            //var resolver = new AutofacDependencyResolver(EngineContext.Current.ContainerManager.Container);
            //// Configure SignalR with the dependency resolver.
            //GlobalHost.DependencyResolver = resolver;


            //// Create the dependency resolver.
            //var resolver = new AutofacDependencyResolver(EngineContext.Current.ContainerManager.Container);

            //// Configure SignalR with the dependency resolver.
            //GlobalHost.DependencyResolver = resolver;

            // Glimpse Autofac
            //EngineContext.Current.ContainerManager.Container.ActivateGlimpse();

            // model binders
            ModelBinders.Binders.DefaultBinder = new SpecterModelBinder();

            // Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new SpecterMetadataProvider();

            //var adminArea = new AdminAreaRegistration();
            //var adminAreaContext = new AreaRegistrationContext(adminArea.AreaName, RouteTable.Routes);
            //adminArea.RegisterArea(adminAreaContext);

            AreaRegistration.RegisterAllAreas();

            //var defaultArea = new DefaultAreaRegistration();
            //var defaultAreaContext = new AreaRegistrationContext(defaultArea.AreaName, RouteTable.Routes);
            //defaultArea.RegisterArea(defaultAreaContext);

            //GlobalConfiguration.Configure(WebApiConfig.Register);

            RouteConfig.RegisterRoutes(RouteTable.Routes, installed);

            if (installed)
            {
                //var profilingEnabled = this.ProfilingEnabled;

                // register our themeable razor view engine we use
                //IViewEngine viewEngine = new RazorViewEngine();
                //if (profilingEnabled)
                //{
                //    // ...and wrap, if profiling is active
                //    viewEngine = new ProfilingViewEngine(viewEngine);
                //    GlobalFilters.Filters.Add(new ProfilingActionFilter());
                //}
                //ViewEngines.Engines.Add(viewEngine);

                // Global filters
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

                // Bundles
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                // register virtual path provider for theme variables
                //HostingEnvironment.RegisterVirtualPathProvider(new ThemeVarsVirtualPathProvider(HostingEnvironment.VirtualPathProvider));
                //BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;

                // register virtual path provider for embedded views
                //var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
                //var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
                //HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

                // start scheduled tasks
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }
            else
            {
                // app not installed

                // Install filter
                //GlobalFilters.Filters.Add(new HandleInstallFilter());
            }
           
            #region For Testing
            //var observableEfRepository = new ObservableEfRepository<SignalR_>(context, query, null, "", null, new SqlParameter("@userGuid", user.UserGuid));
            //var _dataServerSettings = EngineContext.Current.Resolve<Specter.Web.Core.Domain.DataServer.DataServerSettings>();
            //var _settingService = EngineContext.Current.Resolve<ISettingService>();
            //_dataServerSettings.OptionElements = new Dictionary<string, object>();
            //_dataServerSettings.OptionElements.Add("Name", "Test");
            //_settingService.SaveSetting(_dataServerSettings);

            //var cacheManager = EngineContext.Current.Resolve<ICacheManager>("static");
            //cacheManager.Clear();
            //var settings = EngineContext.Current.Resolve<ISettingService>();
            //var users = EngineContext.Current.Resolve<IRepository<User>>();
            //var context = EngineContext.Current.Resolve<IDbContext>();
            //var logRepository = EngineContext.Current.Resolve<IRepository<Log>>();
            //var log = new Log()
            //{
            //    Frequency = 1,
            //    LogLevel =  LogLevel.Error,
            //    ShortMessage = "Test error",
            //    FullMessage = "Test error full message",
            //    IpAddress = "",
            //    User = users.GetById(2),
            //    PageUrl = "pageUrl",
            //    ReferrerUrl = "referrerUrl",
            //    CreatedOnUtc = DateTime.UtcNow,
            //    ContentHash = "contentHash"
            //};
            //logRepository.Insert(log);
            //var genericAttributeRepository = EngineContext.Current.Resolve<IRepository<GenericAttribute>>();
            //var attribute = new GenericAttribute()
            //{
            //    Id = 0,
            //    EntityId = 2,
            //    Key = "User",
            //    KeyGroup = "LanguageId",
            //    Value = "1"
            //};
            //genericAttributeRepository.Insert(attribute);

            //var objectsPosition = EngineContext.Current.Resolve<IRepository<ObjectPosition>>();
            //var objectsIndicator = EngineContext.Current.Resolve<IRepository<ObjectIndicator>>();

            //var settings = EngineContext.Current.Resolve<IRepository<Setting>>();
            //var devices = EngineContext.Current.Resolve<IRepository<Device>>();
            //var devicePositions = EngineContext.Current.Resolve<IRepository<DevicePosition>>();
            //var devicePosition = devicePositions.GetById(27);

            //var deviceTypes = EngineContext.Current.Resolve<IRepository<DeviceType>>();
            //var geoZones = EngineContext.Current.Resolve<IRepository<GeoZone>>();
            //var policies = EngineContext.Current.Resolve<IRepository<Policies>>();
            //var polygonPoints = EngineContext.Current.Resolve<IRepository<PolygonPoint>>();
            ////var userDevices = EngineContext.Current.Resolve<IRepository<UserDevice>>();

            //var roles = EngineContext.Current.Resolve<IRepository<Role>>();

            //var points = EngineContext.Current.Resolve<IRepository<Table_Geo>>();
            //var point = points.GetById(7);
            //var foo = new Foo
            //{
            //    Location = System.Data.Entity.Spatial.DbGeography.FromText(
            //        "Polygon((22.3 77.4, 22.5 77.5, 22.7 77.5, 22.3 77.4))")
            //};
            //var geoZone = geoZones.GetById(8).Polygon;

            //var context = EngineContext.Current.Resolve<IDbContext>();
            //var centerById = context.SqlQuery<string>("SELECT (Polygon.MakeValid()).EnvelopeCenter().ToString() FROM dbo.GeoZone WHERE id = @id",
            //    new System.Data.SqlClient.SqlParameter("@id", 8));

            //var text = new SqlChars(geoZone.AsText().ToCharArray());
            //var param = SqlGeography.STPolyFromText(text, geoZone.CoordinateSystemId);

            //var centerByGeo = context.SqlQuery<string>(string.Format("SELECT ((geography::STPolyFromText('{0}', {1})).MakeValid()).EnvelopeCenter().ToString()",
            //    geoZone.AsText(), geoZone.CoordinateSystemId));

            #endregion For Testing

            #region For testing email
            //var user = EngineContext.Current.Resolve<IUserService>().GetUserById(1);
            //var emailAccountService = EngineContext.Current.Resolve<IEmailAccountService>();
            //var emailAccountSettings = EngineContext.Current.Resolve<EmailSettings>();
            //var emailAccount = emailAccountService.GetEmailAccountById(emailAccountSettings.DefaultEmailAccountId);
            //var queuedEmailService = EngineContext.Current.Resolve<IQueuedEmailService>();
            //var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
            //workflowMessageService.SendUserEmailValidationMessage(user, 1);
            //workflowMessageService.SendUserPasswordRecoveryMessage(user, 1);
            //workflowMessageService.SendUserWelcomeMessage(user, 1);
            #endregion For testing email
        }

        protected void Application_End()
        {
            //SqlDependency.Stop(DataSettings.Current.DataConnectionStringNotify);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //if (authCookie != null)
            //{

            //    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            //    CustomPrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<CustomPrincipalSerializeModel>(authTicket.UserData);
            //    CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
            //    newUser.UserId = (int)serializeModel.Id;
            //    newUser.FirstName = serializeModel.FirstName;
            //    newUser.LastName = serializeModel.LastName;
            //    newUser.Roles = serializeModel.Roles;

            //    HttpContext.Current.User = newUser;
            //}
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //var installed = DataSettings.DatabaseIsInstalled();

            // ignore static resources
            if (WebHelper.IsStaticResourceRequested(this.Request))
                return;

            //_profilingEnabled = this.ProfilingEnabled;

            //if (_profilingEnabled)
            //{
            //    MiniProfiler.Start();
            //}
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            // Don't resolve dependencies from now on.

            // ignore static resources
            if (WebHelper.IsStaticResourceRequested(this.Request))
                return;

            //if (_profilingEnabled)
            //{
            //    // stop mini profiler
            //    MiniProfiler.Stop();
            //}
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // [...]
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            //disable compression (if enabled). More info - http://stackoverflow.com/questions/3960707/asp-net-mvc-weird-characters-in-error-page
            //log error
            LogException(Server.GetLastError());
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;

            if (!DataSettings.DatabaseIsInstalled())
                return;

            try
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentUser);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}
