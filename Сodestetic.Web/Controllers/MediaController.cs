using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Services.Media;
using Codestetic.Web.Services.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Codestetic.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IPictureService _pictureService;
        private readonly IPermissionService _permissionService;

        public MediaController(
            IPictureService pictureService,
            IPermissionService permissionService)
        {
            this._pictureService = pictureService;
            this._permissionService = permissionService;
        }

        [HttpPost]
        public ActionResult AsyncUpload(string type)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            var pictureType = type.ToLowerInvariant().ToEnum<PictureType>(PictureType.entity);

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var FileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                FileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                // Webkit, Mozilla
                stream = Request.InputStream;
                FileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(FileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
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
            if (contentType.StartsWith("image"))
            {
                var picture = _pictureService.InsertPicture(fileBinary, contentType, null, true);
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(
                    new
                    {
                        success = true,
                        pictureId = picture.Id,
                        imageUrl = _pictureService.GetPictureUrl(picture, _pictureService.GetDefaultSize(type), true, pictureType)
                    },
                    "text/plain");
            }
            return Json(
                new
                {
                    success = false,
                },
                "text/plain");
        }

        public ActionResult AsyncSave(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    var fileExtension = Path.GetExtension(file.FileName);

                    //SessionUploadInitialFilesRepository.Add(new UploadInitialFile(fileName, file.ContentLength, fileExtension));

                    // The files are not actually saved in this demo
                    // file.SaveAs(physicalPath);
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        [HttpPost]
        public ActionResult GetNextSystemMarker(long iconId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
                return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");
            var icons = _pictureService.GetSystemMarkers();
            var index = icons.Where(i => i.Id == iconId).FirstOrDefault();
            if (index != null)
            {
                var icon = icons.GetNext(index, true);
                return Json(
                    new
                    {
                        success = true,
                        iconId = icon.Id,
                        src = icon.Url
                    },
                    "text/plain");
            }
            return Json(
                new
                {
                    success = true,
                    iconId = icons[0].Id,
                    src = icons[0].Url
                },
                "text/plain");
        }

        [HttpPost]
        public ActionResult GetMarkerById(long iconId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
                return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            var icon = _pictureService.GetSystemMarkers().Where(i => i.Id == iconId).FirstOrDefault();
            var iconShadow = _pictureService.CreateShadow(icon.Id);
            if (icon != null)
            {
                return Json(new 
                {
                    success = true,
                    iconId = icon.Id,
                    imageUrl = icon.Url,
                    size = icon.Size,
                    //test = new KeyValuePair<long, bool>(1, true)
                }, "text/plain");
            }
            return Json(new 
            {
                success = true,
                iconId = 0,
                imageUrl = "",
                size = Size.Empty
            }, "text/plain");
        }

        [HttpPost]
        public ActionResult GetPhotoById(long photoId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
                return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");
            var url = _pictureService.GetPictureUrl(photoId, 100, true, PictureType.photo);
            var urlBigSize = _pictureService.GetPictureUrl(photoId, 200, true, PictureType.photo);
            return Json(new 
            {
                success = true,
                imageUrl = url,
                imageUrlBigSize = urlBigSize
            }, "text/plain");
        }

        public JsonResult GetMarkers()
        {
            Func<IPictureService, Marker, object> GetDeltaPosition = (service, icon) =>
            {
                var avalableShadow = icon.ShadowMarkerId.Value != 0;
                var shadow = avalableShadow ? service.GetMarkerById(icon.ShadowMarkerId.Value) : null;
                return new
                {
                    id = icon.Id,
                    marker = new {
                        url = _pictureService.GetMarkerUrl(icon.Id),
                        width = icon.Width,
                        height = icon.Height,
                        x = icon.AnchorX,
                        y = icon.AnchorY
                    },
                    shadow = avalableShadow ? new {
                        url = _pictureService.GetMarkerUrl(shadow.Id),
                        width = shadow.Width,
                        height = shadow.Height,
                        x = shadow.AnchorX,
                        y = shadow.AnchorY
                    } : null,
                };
            };
            var icons = _pictureService.GetMarkers().Select(x => GetDeltaPosition(_pictureService, x));

            return Json(icons, JsonRequestBehavior.AllowGet);
        }

        #region Helpers
        #endregion Helpers
    }
}