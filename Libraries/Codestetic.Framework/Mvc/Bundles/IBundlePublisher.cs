using System.Web.Optimization;

namespace Codestetic.Web.Framework.Mvc.Bundles
{
    public interface IBundlePublisher
    {
        void RegisterBundles(BundleCollection bundles);
    }
}
