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
    public interface ICommonServices
    {
        ICacheManager Cache { get; }
        IDbContext DbContext { get; }
        IWebHelper WebHelper { get; }
        IWorkContext WorkContext { get; }
        IEventPublisher EventPublisher { get; }
        ILocalizationService Localization { get; }
        IUserActivityService UserActivity { get; }
        INotifier Notifier { get; }
        IPermissionService Permissions { get; }
    }
}
