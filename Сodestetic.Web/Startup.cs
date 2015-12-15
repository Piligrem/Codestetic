using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Codestetic.Web.Services.SignalR;

[assembly: OwinStartup(typeof(Codestetic.Web.Startup))]

namespace Codestetic.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(60);
            app.MapSignalR();
        }
    }
}
