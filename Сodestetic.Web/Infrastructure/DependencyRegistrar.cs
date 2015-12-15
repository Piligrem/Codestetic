using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Core.Infrastructure.DependencyManagement;
//using Specter.Web.Data.Setup;
using Codestetic.Web.Controllers;
using Codestetic.Web.Framework.UI;
//using Specter.Web.Infrastructure.Installation;

namespace Codestetic.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //we cache presentation models between requests
            //builder.RegisterType<BlogController>().WithStaticCache();
            //builder.RegisterType<CatalogController>().WithStaticCache();
            //builder.RegisterType<CountryController>().WithStaticCache();
            builder.RegisterType<CommonController>().WithStaticCache();
            //builder.RegisterType<NewsController>().WithStaticCache();
            //builder.RegisterType<PollController>().WithStaticCache();
            //builder.RegisterType<ShoppingCartController>().WithStaticCache();
            builder.RegisterType<TopicController>().WithStaticCache();

            builder.RegisterType<DefaultWidgetSelector>().As<IWidgetSelector>().WithStaticCache().InstancePerRequest();

            // installation localization service
            //builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerHttpRequest();

            // register app languages for installation
            //builder.RegisterType<EnUSSeedData>()
            //    .As<InvariantSeedData>()
            //    .WithMetadata<InstallationAppLanguageMetadata>(m =>
            //    {
            //        m.For(em => em.Culture, "en-US");
            //        m.For(em => em.Name, "English");
            //        m.For(em => em.UniqueSeoCode, "en");
            //        m.For(em => em.FlagImageFileName, "us.png");
            //    })
            //    .InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}