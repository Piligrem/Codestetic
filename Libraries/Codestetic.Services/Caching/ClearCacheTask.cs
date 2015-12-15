using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Services.Tasks;

namespace Codestetic.Web.Services.Caching
{
    /// <summary>
    /// Clear cache scheduled task implementation
    /// </summary>
    public partial class ClearCacheTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var cacheManager = EngineContext.Current.Resolve<ICacheManager>("static");
            cacheManager.Clear();
        }
    }
}
