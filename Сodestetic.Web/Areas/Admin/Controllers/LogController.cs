using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Codestetic.Web.Areas.Admin.Models.Logging;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Domain.Logging;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Areas.Admin.Infrastructure;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LogController : AdminControllerBase
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;

        private static readonly Dictionary<LogLevel, string> s_logLevelHintMap = new Dictionary<LogLevel, string> 
        { 
            { LogLevel.Fatal, "inverse" },
            { LogLevel.Error, "important" },
            { LogLevel.Warning, "warning" },
            { LogLevel.Information, "info" },
            { LogLevel.Debug, "default" }
        };

        public LogController(IWorkContext workContext,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService)
        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = new LogSearchListModel();
            model.AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList();
            model.AvailableLogLevels.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }


        //public ActionResult LogList([DataSourceRequest] DataSourceRequest request) //GridCommand command, LogListModel model)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
        //        return AccessDeniedView();

        //    DateTime? createdOnFromValue = (model.CreatedOnFrom == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

        //    DateTime? createdToFromValue = (model.CreatedOnTo == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

        //    LogLevel? logLevel = model.LogLevelId > 0 ? (LogLevel?)(model.LogLevelId) : null;


        //    var logItems = Logger.GetAllLogs(createdOnFromValue, createdToFromValue, model.Message,
        //        logLevel, command.Page - 1, command.PageSize, model.MinFrequency);

        //    var gridModel = new GridModel<LogModel>
        //    {
        //        Data = logItems.Select(x =>
        //        {
        //            var logModel = new LogModel()
        //            {
        //                Id = x.Id,
        //                LogLevelHint = s_logLevelHintMap[x.LogLevel],
        //                LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
        //                ShortMessage = x.ShortMessage.Length > 100 ? x.ShortMessage.Substring(0, 99) + "..." : x.ShortMessage,
        //                FullMessage = x.FullMessage,
        //                IpAddress = x.IpAddress,
        //                UserId = x.UserId,
        //                UserEmail = x.User != null ? x.User.Email : null,
        //                PageUrl = x.PageUrl,
        //                ReferrerUrl = x.ReferrerUrl,
        //                CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).Value,
        //                Frequency = x.Frequency,
        //                ContentHash = x.ContentHash
        //            };

        //            if (x.UpdatedOnUtc.HasValue)
        //                logModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc.Value, DateTimeKind.Utc);

        //            return logModel;
        //        }),
        //        Total = logItems.TotalCount
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}
        [HttpPost]
        public JsonResult LogList([DataSourceRequest] DataSourceRequest request, LogSearchListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return null;

            var result = PrepereLogListModel(model);
            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public JsonResult LogSearchList([DataSourceRequest] DataSourceRequest request, LogSearchListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return null;

            var result = PrepereLogListModel(model);

            return Json(result.ToDataSourceResult(request));
        }
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public JsonResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return Json(new { Success = false }); ;

			Logger.ClearLog();
            var notify = _localizationService.GetResource("Admin.System.Log.Cleared");
            NotifySuccess(notify);
            return Json(new { Success = true, Notify = notify });
            //return RedirectToAction("List");
        }

        public ActionResult View(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

			var log = Logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

            var model = new LogModel()
            {
                Id = log.Id,
                LogLevelHint = s_logLevelHintMap[log.LogLevel],
                LogLevel = log.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                UserId = log.UserId,
                UserEmail = log.User != null ? log.User.Email : null,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc).Value,
				Frequency = log.Frequency,
				ContentHash = log.ContentHash
            };

			if (log.UpdatedOnUtc.HasValue)
				model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(log.UpdatedOnUtc.Value, DateTimeKind.Utc);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

			var log = Logger.GetLogById(id);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction("List");

			Logger.DeleteLog(log);


            NotifySuccess(_localizationService.GetResource("Admin.System.Log.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<long> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
				var logItems = Logger.GetLogByIds(selectedIds.ToArray());
                foreach (var logItem in logItems)
					Logger.DeleteLog(logItem);
            }

            return Json(new { Result = true});
        }

        #region Utilities
        [NonAction]
        protected IEnumerable<LogListModel> PrepereLogListModel(LogSearchListModel model)
        {
            IEnumerable<LogListModel> result;
            IEnumerable<Log> logItems;

            if (!model.CreatedOnFrom.HasValue && !model.CreatedOnTo.HasValue &&
                string.IsNullOrEmpty(model.Message) && model.LogLevelId == 0 && model.MinFrequency == 0)
            {
                logItems = Logger.GetAllLogs();
            }
            else
            {
                var fromUtc = model.CreatedOnFrom.HasValue ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value) : null;
                var toUtc = model.CreatedOnTo.HasValue ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value) : null;

                logItems = Logger.GetAllLogs(fromUtc, toUtc, model.Message, (LogLevel?)model.LogLevelId, 0, int.MaxValue, model.MinFrequency);
            }
            result = logItems.Select(x => new LogListModel() 
            {
                Id = x.Id,
                LogLevelHint = s_logLevelHintMap[x.LogLevel],
                LogLevel = x.LogLevel.GetLocalizedEnum(_localizationService, _workContext),
                ShortMessage = x.ShortMessage.Length > 100 ? x.ShortMessage.Substring(0, 99) + "..." : x.ShortMessage,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).Value,
                Frequency = x.Frequency,
                UpdatedOn = x.UpdatedOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc.Value, DateTimeKind.Utc) : null
            });
            return result;
        }
        #endregion Utilities
    }
}
