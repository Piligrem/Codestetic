using System;
using System.Collections.Generic;
using System.Linq;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Services.Logging;

namespace Codestetic.Web.Services
{
    public class CommonServices : ICommonServices
    {
        private readonly Lazy<ICacheManager> _cache;
        private readonly Lazy<IDbContext> _dbContext;
        private readonly Lazy<IWebHelper> _webHelper;
        private readonly Lazy<IWorkContext> _workContext;
        private readonly Lazy<IEventPublisher> _eventPublisher;
        private readonly Lazy<ILocalizationService> _localization;
        private readonly Lazy<IUserActivityService> _userActivity;
        private readonly Lazy<INotifier> _notifier;
        private readonly Lazy<IPermissionService> _permissions;

        public CommonServices(
            Func<string, Lazy<ICacheManager>> cache,
            Lazy<IDbContext> dbContext,
            Lazy<IWebHelper> webHelper,
            Lazy<IWorkContext> workContext,
            Lazy<IEventPublisher> eventPublisher,
            Lazy<ILocalizationService> localization,
            Lazy<IUserActivityService> userActivity,
            Lazy<INotifier> notifier,
            Lazy<IPermissionService> permissions)
        {
            this._cache = cache("static");
            this._dbContext = dbContext;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._eventPublisher = eventPublisher;
            this._localization = localization;
            this._userActivity = userActivity;
            this._notifier = notifier;
            this._permissions = permissions;
        }

        public ICacheManager Cache { get { return _cache.Value; } }
        public IDbContext DbContext { get { return _dbContext.Value; } }
        public IWebHelper WebHelper { get { return _webHelper.Value; } }
        public IWorkContext WorkContext { get { return _workContext.Value; } }
        public IEventPublisher EventPublisher { get { return _eventPublisher.Value; } }
        public ILocalizationService Localization { get { return _localization.Value; } }
        public IUserActivityService UserActivity { get { return _userActivity.Value; } }
        public INotifier Notifier { get { return _notifier.Value; } }
        public IPermissionService Permissions { get { return _permissions.Value; } }
    }
}
