using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Spatial;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Concurrent;

using Codestetic.Web.Services.SignalR;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using System.Threading.Tasks;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Core.Domain.Logging;

namespace Codestetic.Web.Controllers
{
    [Authorize]
    public class LoggingController : BaseController
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IUserActivityService _userActivityService;
        #endregion Fields

        #region Constructors
        public LoggingController(
            ILogger logger, IWorkContext workContext,
            IUserActivityService userActivityService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._userActivityService = userActivityService;
        }
        #endregion Constructors

        #region Methods
        
        [HttpPost]
        public JsonResult ErrorLog(string message, string stack)
        {
            if (_workContext.CurrentUser.IsRegistered())
                _logger.InsertLog(LogLevel.Error, message, stack, _workContext.CurrentUser);
            return Json(true);
        }
        [HttpPost]
        public JsonResult ActivityLog(string systemKeyword, string message)
        {
            if (_workContext.CurrentUser.IsRegistered())
                _userActivityService.InsertActivity(systemKeyword, message);
            return Json(true);
        }
        #endregion Methods
    }
}
