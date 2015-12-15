using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Routing;
using System.Web.Http.OData.Routing.Conventions;
using System.Web.Http.Cors;
using System.Web.Http.OData.Extensions;
using Specter.Web.Framework.WebApi.OData;
using Specter.Web.Core.Infrastructure;
using Specter.Web.Framework.WebApi.Configuration;

namespace Specter.Web.Framework.WebApi
{   
    public class WebApiStartupTask : IStartupTask
    {
        public void Execute()
        {
			var config = GlobalConfiguration.Configuration;

			var configBroadcaster = new WebApiConfigurationBroadcaster()
			{
				ModelBuilder = new ODataConventionModelBuilder(),
				RoutingConventions = ODataRoutingConventions.CreateDefault(),
				Routes = config.Routes
			};

			config.DependencyResolver = new AutofacWebApiDependencyResolver();

			config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
			config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "json", "application/json"));
			config.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "xml", "application/xml"));

			var queryAttribute = new WebApiQueryableAttribute()
			{
				MaxTop = WebApiGlobal.MaxTop
			};
            config.AddODataQueryFilter(queryAttribute);
			//config.EnableQuerySupport(queryAttribute);

			var corsAttribute = new EnableCorsAttribute("*", "*", "*", WebApiGlobal.Header.CorsExposed);
			config.EnableCors(corsAttribute);

			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

			var configPublisher = EngineContext.Current.Resolve<IWebApiConfigurationPublisher>();
			configPublisher.Configure(configBroadcaster);

			//config.Services.Insert(typeof(ModelBinderProvider), 0,
			//	new SimpleModelBinderProvider(typeof(Address), new AddressModelBinder()));

			try
			{
				if (!config.Routes.ContainsKey(WebApiGlobal.RouteNameDefaultApi))
				{
					config.Routes.MapHttpRoute(WebApiGlobal.RouteNameDefaultApi, "api/{version}/{controller}/{id}",
						new { version = "v1", controller = "Home", id = RouteParameter.Optional });
				}
			}
			catch (Exception) { }

			try
			{
				if (!config.Routes.ContainsKey(WebApiGlobal.RouteNameDefaultOdata))
				{
                    config.Routes.MapODataServiceRoute(WebApiGlobal.RouteNameDefaultOdata, WebApiGlobal.MostRecentOdataPath,
                        configBroadcaster.ModelBuilder.GetEdmModel(), new DefaultODataPathHandler(), configBroadcaster.RoutingConventions);
                    //config.Routes.MapODataRoute(WebApiGlobal.RouteNameDefaultOdata, WebApiGlobal.MostRecentOdataPath,
                    //    configBroadcaster.ModelBuilder.GetEdmModel(), new DefaultODataPathHandler(), configBroadcaster.RoutingConventions);
				}
			}
			catch (Exception) { }
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
