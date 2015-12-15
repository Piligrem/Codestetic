using System.Web.Optimization;

namespace Codestetic.Web.Framework.Mvc.Bundles
{
    public interface IBundleProvider
    {
        void RegisterBundles(BundleCollection bundles);

        int Priority { get; }
    }
}
