using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Kendo.Mvc;
using System.Collections.Generic;
using Codestetic.Web.Areas.Admin.Models.Localization;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Core.Plugins;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Infrastructure;
using Codestetic.Web.Areas.Admin.Infrastructure;
using Kendo.Mvc.UI;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public partial class LanguageController : AdminControllerBase
    {
        #region Fields
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IPluginFinder _pluginFinder;
        #endregion Fields

        #region Constructors
        public LanguageController(ILanguageService languageService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWebHelper webHelper,
            AdminAreaSettings adminAreaSettings,
            IPluginFinder pluginFinder)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._permissionService = permissionService;
            this._webHelper = webHelper;
            this._adminAreaSettings = adminAreaSettings;
            this._pluginFinder = pluginFinder;
        }

        #endregion Constructors

        #region Utilities
        [NonAction]
        private void PrepareFlagsModel(LanguageModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.FlagFileNames = System.IO.Directory
                .EnumerateFiles(_webHelper.MapPath("~/Content/Images/flags/"), "*.png", SearchOption.TopDirectoryOnly)
                .Select(System.IO.Path.GetFileName)
                .ToList();
        }
        #endregion Utilities

        #region Languages
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var languages = _languageService.GetAllLanguages(true);
            var gridModel = new GridModel<LanguageModel>
            {
                Data = languages.Select(x => x.ToModel()),
                Total = languages.Count()
            };
            return View(gridModel);
        }
        [HttpPost]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var languages = _languageService.GetAllLanguages(true);
            var gridModel = new GridModel<LanguageModel>
            {
                Data = languages.Select(x => x.ToModel()),
                Total = languages.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var model = new LanguageModel();
            //flags
            PrepareFlagsModel(model);

            //default values
            //model.Published = true;
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Create(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                _languageService.InsertLanguage(language);

                var plugins = _pluginFinder.GetPluginDescriptors(true);
                var filterLanguages = new List<Language>() { language };

                foreach (var plugin in plugins)
                {
                    _localizationService.ImportPluginResourcesFromXml(plugin, null, false, filterLanguages);
                }

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }

            //flags
            PrepareFlagsModel(model);

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = language.ToModel();

            //flags
            PrepareFlagsModel(model);

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Edit(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(model.Id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = _languageService.GetAllLanguages();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id &&
                    !model.Published)
                {
                    NotifyError("At least one published language is required.");
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                _languageService.UpdateLanguage(language);

                //notification
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //flags
            PrepareFlagsModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = _languageService.GetAllLanguages();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                NotifyError("At least one published language is required.");
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            _languageService.DeleteLanguage(language);

            //notification
            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Deleted"));
            return RedirectToAction("List");
        }
        #endregion Languages

        #region Resources
        //[HttpPost]
        //public ActionResult Paging_Orders([DataSourceRequest] DataSourceRequest request)
        //{
        //    return Json(GetOrders().ToDataSourceResult(request));
        //}
        public ActionResult Resources( long languageId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true)
                .Select(x => new SelectListItem
                {
                    Selected = (x.Id.Equals(languageId)),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            var language = _languageService.GetLanguageById(languageId);
            ViewBag.LanguageId = languageId;
            ViewBag.LanguageName = language.Name;

            var resources = _localizationService
                .GetResourceValues(languageId, true)
                .Where(x => x.Key != "!!___EOF___!!" && x.Value != null)
                .OrderBy(x => x.Key)
                .ToList();

            var gridModel = new GridModel<LanguageResourceModel>
            {
                Data = resources
                    //.Take(_adminAreaSettings.GridPageSize)
                    .Select(x => new LanguageResourceModel()
                    {
                        LanguageId = languageId,
                        LanguageName = language.Name,
                        Id = x.Value.Item1,
                        Name = x.Key,
                        Value = x.Value.Item2.EmptyNull(),
                    }),
                Total = resources.Count
            };
            return View(gridModel);
        }
        [HttpPost]
        public ActionResult Resources(long languageId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(languageId);

            var resources = _localizationService
                .GetResourceValues(languageId, true)
                .OrderBy(x => x.Key)
                .Where(x => x.Key != "!!___EOF___!!" && x.Value != null)
                .Select(x => new LanguageResourceModel()
                {
                    LanguageId = languageId,
                    LanguageName = language.Name,
                    Id = x.Value.Item1,
                    Name = x.Key,
                    Value = x.Value.Item2.EmptyNull(),
                })
                .ForCommand(command)
                .ToList();

            var model = new GridModel<LanguageResourceModel>
            {
                Data = resources.PagedForCommand(command),
                Total = resources.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        public ActionResult ResourceUpdate(LanguageResourceModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var resource = _localizationService.GetLocaleStringResourceById(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = _localizationService.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return Content(string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
                }
            }

            resource.ResourceName = model.Name;
            resource.ResourceValue = model.Value;
            resource.IsTouched = true;
            _localizationService.UpdateLocaleStringResource(resource);

            return Resources(model.LanguageId, command);
        }

        public ActionResult ResourceAdd(long id, LanguageResourceModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var res = _localizationService.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
            if (res == null)
            {
                var resource = new LocaleStringResource { LanguageId = id };
                resource.ResourceName = model.Name;
                resource.ResourceValue = model.Value;
                resource.IsTouched = true;
                _localizationService.InsertLocaleStringResource(resource);
            }
            else
            {
                return Content(string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.Name));
            }
            return Resources(id, command);
        }

        public ActionResult ResourceDelete(long id, int languageId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var resource = _localizationService.GetLocaleStringResourceById(id);
            if (resource == null)
                throw new ArgumentException("No resource found with the specified id");
            _localizationService.DeleteLocaleStringResource(resource);

            return Resources(languageId, command);
        }
        #endregion Resources

        #region Export/Import
        public ActionResult ExportXml(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            try
            {
                var xml = _localizationService.ExportResourcesToXml(language);
                return new XmlDownloadResult(xml, "language-pack-{0}.xml".FormatInvariant(language.UniqueSeoCode));
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ImportXml(long id, FormCollection form, ImportModeFlags mode, bool updateTouched)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                var file = Request.Files["importxmlfile"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var sr = new StreamReader(file.InputStream, Encoding.UTF8))
                    {
                        string content = sr.ReadToEnd();
                        _localizationService.ImportResourcesFromXml(language, content, mode: mode, updateTouchedResources: updateTouched);
                    }

                }
                else
                {
                    NotifyError(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                NotifyError(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }
        }
        #endregion Export/Import
    }
}
