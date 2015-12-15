using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using Codestetic.Web.Core.Plugins;

namespace Codestetic.Web.Framework.Mvc.Bundles
{
    public class BundlePublisher : IBundlePublisher
    {
        private readonly ITypeFinder _typeFinder;

        public BundlePublisher(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public void RegisterBundles(BundleCollection bundles)
        {
            var bundleProviderTypes = _typeFinder.FindClassesOfType<IBundleProvider>();
            var bundleProviders = new List<IBundleProvider>();
            foreach (var providerType in bundleProviderTypes)
            {
                if (!PluginManager.IsActivePluginAssembly(providerType.Assembly))
                {
                    continue;
                }
                
                var provider = Activator.CreateInstance(providerType) as IBundleProvider;
                bundleProviders.Add(provider);
            }

            bundleProviders = bundleProviders.OrderByDescending(bp => bp.Priority).ToList();
            bundleProviders.Each(bp => bp.RegisterBundles(bundles));
        }
    }
}
