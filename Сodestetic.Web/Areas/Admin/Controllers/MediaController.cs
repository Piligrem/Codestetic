using System;
using System.Collections.Generic;
using System.Drawing;
using Point2D = System.Drawing.Point;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Codestetic.Web.Areas.Admin.Models.Devices;
using Codestetic.Web.Areas.Admin.Models.Media;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Services.Media;
using Marker = Codestetic.Web.Core.Domain.Media.Marker;
using System.IO;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services;
using Codestetic.Web.Services.Logging;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class MediaController : AdminControllerBase
    {
        #region Fields
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPermissionService _permissionService;
        private readonly ICommonServices _commonService;
        private readonly IUserActivityService _userActivityService;
        #endregion

        #region Constructors
        public MediaController(IPictureService pictureService,
            MediaSettings mediaSettings, ILocalizationService localizationService,
            IPermissionService permissionService, ICommonServices commonService,
            IUserActivityService userActivityService)
        {
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._mediaSettings = mediaSettings;
            this._permissionService = permissionService;
            this._commonService = commonService;
            this._userActivityService = userActivityService;
        }
        #endregion Constructors

        #region Methods
        public ActionResult Index()
        {
            return View("Markers");
        }

        public ActionResult Markers()
        {            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            return View(GetListMarkerModel());
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Markers(MarkerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _commonService.WorkContext.CurrentUser;
            var isNew = false;

            if (ModelState.IsValid)
            {
                var icon = _pictureService.GetMarkerById(model.Id);

                if (!icon.IsSystem)
                {
                    isNew = icon.IsNew;

                    icon.AnchorX = model.Anchor.X;
                    icon.AnchorY = model.Anchor.Y;
                    icon.IsSystem = model.IsSystem;
                    icon.IsRetina = model.IsRetina;
                    icon.IsNew = false;
                    icon.ShadowMarkerId = model.ShadowMarker != null ? model.ShadowMarker.Id : 0;
                    _pictureService.UpdateMarker(icon);

                    model.Url = _pictureService.GetMarkerUrl(model.Id);

                    if (model.ShadowMarker != null)
                    {
                        var shadow = _pictureService.GetMarkerById(icon.ShadowMarkerId.Value);

                        shadow.ShadowMarkerId = null;
                        shadow.AnchorX = model.ShadowMarker.Anchor.X;
                        shadow.AnchorY = model.ShadowMarker.Anchor.Y;
                        shadow.IsSystem = model.IsSystem;
                        shadow.IsRetina = model.IsRetina;
                        shadow.IsNew = false;
                        _pictureService.UpdateMarker(shadow);
                        model.ShadowMarker.Url = _pictureService.GetMarkerUrl(model.ShadowMarker.Id);
                    }

                    //activity log
                    if (isNew)
                    {
                        _userActivityService.InsertActivity("AddNewMarker", _localizationService.GetResource("ActivityLog.AddNewMarker"), icon.Id);
                        NotifySuccess(_localizationService.GetResource("Admin.Media.Added"));
                    }
                    else
                    {
                        _userActivityService.InsertActivity("UpdateMarker", _localizationService.GetResource("ActivityLog.UpdateMarker"), icon.Id);
                        NotifySuccess(_localizationService.GetResource("Admin.Media.Updated"));
                    }

                    _pictureService.DeleteMarkers(user.Id);
                    return continueEditing ? View(GetListMarkerModel(model)) : View(GetListMarkerModel());
                }
                else
                {
                    NotifyWarning(_localizationService.GetResource("Admin.Media.NotPossibleChangeSystemMarker"));
                }
            }
            _pictureService.DeleteMarkers(user.Id);
            return RedirectToAction("Markers");
        }

        public JsonResult GetMarkers()
        {
            
            var icons = _pictureService.GetMarkers().Select(x => new 
            {
                Id = x.Id,
                Url = _pictureService.GetMarkerUrl(x.Id),
                ShadowUrl = _pictureService.GetMarkerUrl(x.ShadowMarkerId.Value),
                Size = GetDeltaPosition(_pictureService, x)
            });

            return Json(icons, JsonRequestBehavior.AllowGet);
        }
        
        #region Delete icon
        public ActionResult DeleteMarkers()
        {
            var user = _commonService.WorkContext.CurrentUser;
            _pictureService.DeleteMarkers(user.Id);
            return Json(new
            {
                success = true,
            });
        }
        [HttpPost]
        public ActionResult DeleteMarker(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();
            var icon = _pictureService.GetMarkerById(id);

            if (icon != null)
            {
                if (!icon.IsSystem)
                {
                    _pictureService.DeleteMarker(icon);
                    NotifySuccess(_localizationService.GetResource("Admin.Media.Deleted"));
                }
                else
                {
                    NotifyWarning(_localizationService.GetResource("Admin.Media.CannotDeleteSystemMarker"));
                }
            }

            return RedirectToAction("Markers");
        }
        #endregion Delete icon

        [HttpPost]
        public ActionResult AsyncUpload()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
                return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            var user = _commonService.WorkContext.CurrentUser;

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var size = _pictureService.GetPictureSize(fileBinary);
            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            #region Test on size and mime type
            if (size.Width > _mediaSettings.DeviceMarkerMaxSize ||
                size.Height > _mediaSettings.DeviceMarkerMaxSize ||
                fileExtension != ".png")
            {
                return Json(
                    new
                    {
                        success = false,
                        message = new
                        {
                            type = NotifyType.Success.ToString().ToLowerInvariant(),
                            message = _localizationService.GetResource("Admin.Media.ImageIsLargeOrNotPng").FormatInvariant(_mediaSettings.DeviceMarkerMaxSize)
                        }
                    },
                    "text/plain");
            }
            #endregion Test on size and mime type

            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            #region Get mimetype
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = "image/tiff";
                        break;
                    default:
                        break;
                }
            }
            #endregion Get mimetype

            #region Success
            if (contentType.StartsWith("image"))
            {
                var icon = _pictureService.InsertMarker(fileBinary, contentType, fileName, true, user.Id);
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(
                    new
                    {
                        success = true,
                        iconId = icon.Id,
                        name = icon.FileName,
                        imageUrl = _pictureService.GetMarkerUrl(icon.Id),
                        size = new Size(icon.Width, icon.Height)
                    },
                    "text/plain");
            }
            #endregion Success

            return Json(
                new
                {
                    success = false,
                },
                "text/plain");
        }
        #endregion Methods

        #region Helpers
        [NonAction]
        private ListMarkerModel GetListMarkerModel(MarkerModel item = null)
        {
            var model = new ListMarkerModel();
            var icons = _pictureService.GetMarkers();
            var maxWidth = 0;
            var maxHeight = 0;
            try
            {
                foreach (var icon in icons)
                {
                    var shadowMarker = _pictureService.GetShadowMarker(icon.Id);
                    var iconModel = new MarkerModel()
                    {
                        Id = icon.Id,
                        Url = _pictureService.GetMarkerUrl(icon.Id),
                        Anchor = new AnchorModel(icon.AnchorX, icon.AnchorY),
                        IsSystem = icon.IsSystem,
                        IsRetina = icon.IsRetina,
                        ShadowMarker = shadowMarker != null ? new MarkerModel()
                        {
                            Id = shadowMarker.Id,
                            Url = _pictureService.GetMarkerUrl(shadowMarker.Id),
                            Anchor = new AnchorModel(shadowMarker.AnchorX, shadowMarker.AnchorY),
                            IsSystem = icon.IsSystem,
                            IsRetina = icon.IsRetina
                        } : null
                    };
                    if (maxWidth < icon.Width) maxWidth = icon.Width;
                    if (maxHeight < icon.Height) maxHeight = icon.Height;
                    model.Markers.Add(iconModel);
                }
                
                model.CurrentMarker = new MarkerModel();
                model.CurrentMarker.ShadowMarker = new MarkerModel();

                if (item != null)
                {
                    model.CurrentMarker = item;
                    if (model.CurrentMarker.ShadowMarker == null)
                    {
                        model.CurrentMarker.ShadowMarker = new MarkerModel();
                    }
                }
            }
            catch (Exception ex) 
            { 
            }
            model.MarkerMaxSize = new Size(_mediaSettings.DeviceMarkerMaxSize, _mediaSettings.DeviceMarkerMaxSize);
            return model;
        }

        private Func<IPictureService, Marker, object> GetDeltaPosition = (service, icon) =>
        {
            if (icon.ShadowMarkerId.Value != 0)
            {
                var shadow = service.GetMarkerById(icon.ShadowMarkerId.Value);
                return new
                {
                    width = icon.Width,
                    height = icon.Height,
                    x = icon.AnchorX - shadow.AnchorX,
                    y = icon.AnchorY - shadow.AnchorY
                };
            }
            return new { x = 0, y = 0 };
        };
        #endregion Helpers
    }
}