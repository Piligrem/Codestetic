using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Codestetic.Web.Areas.Admin.Models.Directory;
using Codestetic.Web.Areas.Admin.Models.Tasks;
using Codestetic.Web.Core.Domain.Directory;
using Codestetic.Web.Core.Domain.Tasks;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.Directory;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Services.Tasks;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Controllers;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public partial class ScheduleTaskController : AdminControllerBase
    {
        #region Fields
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        #endregion Fields

        #region Constructors

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService, IPermissionService permissionService,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper)
        {
            this._scheduleTaskService = scheduleTaskService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
        }

        #endregion

        #region Utility

        [NonAction]
        protected ScheduleTaskModel PrepareScheduleTaskModel(ScheduleTask task)
        {
            var model = new ScheduleTaskModel()
            {
                Id = task.Id,
                Name = _localizationService.GetResource(task.Name),
                Seconds = task.Seconds,
                Enabled = task.Enabled,
                StopOnError = task.StopOnError,
                LastStartUtc = task.LastStartUtc.HasValue ? string.Format("{0:dd-MM-yyyy hh:mm:ss}", _dateTimeHelper.ConvertToUserTime(task.LastStartUtc.Value, DateTimeKind.Utc).Value) : "",
                LastEndUtc = task.LastEndUtc.HasValue ? string.Format("{0:dd-MM-yyyy hh:mm:ss}", _dateTimeHelper.ConvertToUserTime(task.LastEndUtc.Value, DateTimeKind.Utc).Value) : "",
                LastSuccessUtc = task.LastSuccessUtc.HasValue ? string.Format("{0:dd-MM-yyyy hh:mm:ss}", _dateTimeHelper.ConvertToUserTime(task.LastSuccessUtc.Value, DateTimeKind.Utc).Value) : "",
            };
            return model;
        }

        #endregion

        #region Methods
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List([DataSourceRequest]DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            var model = _scheduleTaskService.GetAllTasks(true)
                .Select(PrepareScheduleTaskModel)
                .ToList();

            return Json(model.ToDataSourceResult(request));
        }

        public ActionResult TaskUpdate(ScheduleTaskModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var scheduleTask = _scheduleTaskService.GetTaskById(model.Id);
            if (scheduleTask == null)
                return Content("Schedule task cannot be loaded");

            scheduleTask.Name = model.Name;
            scheduleTask.Seconds = model.Seconds;
            scheduleTask.Enabled = model.Enabled;
            scheduleTask.StopOnError = model.StopOnError;
            _scheduleTaskService.UpdateTask(scheduleTask);

            //return List(command);
            return null;
        }

        #endregion
    }
}
