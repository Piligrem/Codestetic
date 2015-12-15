using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Point2D = System.Drawing.Point;
using Marker = Codestetic.Web.Core.Domain.Media.Marker;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using ImageResizer;
using ImageResizer.Configuration;
using Codestetic.Web.Core;
using Codestetic.Web.Core.IO;
using Codestetic.Web.Core.Data;
//using Specter.Web.Core.Domain.Catalog;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Core.Logging;
//using Specter.Web.Services.Seo;
using Codestetic.Web.Utilities;
using System.Web.Hosting;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Domain.Common;

namespace Codestetic.Web.Services.Media
{   
    /// <summary>
    /// Picture service
    /// </summary>
    public partial class PictureService : IPictureService
    {
        #region Const
        private const int MULTIPLE_THUMB_DIRECTORIES_LENGTH = 4;
        private const string DEVICE_ICONS_ALL_KEY = "specter.device.markers.all";
        #endregion
        
        #region Fields
        private static readonly object s_lock = new object();
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<Marker> _markerRepository;
        //private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly IImageResizerService _imageResizerService;
        private readonly IImageCache _imageCache;
        private readonly ICacheManager _cacheManager;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pictureRepository">Picture repository</param>
        /// <param name="productPictureRepository">Product picture repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="logger">Logger</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="mediaSettings">Media settings</param>
        public PictureService(
            IRepository<Picture> pictureRepository, IRepository<Marker> markerRepository,
            //IRepository<ProductPicture> productPictureRepository,
            ISettingService settingService, 
            IWebHelper webHelper,
            ILogger logger, 
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            IImageResizerService imageResizerService,
            ICacheManager cacheManager,
            CommonSettings commonSettings,
            IImageCache imageCache)
        {
            this._pictureRepository = pictureRepository;
            this._markerRepository = markerRepository;
            //this._productPictureRepository = productPictureRepository;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._logger = logger;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._imageResizerService = imageResizerService;
            this._imageCache = imageCache;
            this._cacheManager = cacheManager;
            this._commonSettings = commonSettings;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Save picture on file system
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        protected virtual void SavePictureInFile(long pictureId, byte[] pictureBinary, string mimeType)
        {
            string lastPart = MimeTypes.MapMimeTypeToExtension(mimeType);
            string FileName = string.Format("{0}-0.{1}", pictureId.ToString("0000000"), lastPart);
            File.WriteAllBytes(GetPictureLocalPath(FileName), pictureBinary);
        }

        /// <summary>
        /// Delete a picture on file system
        /// </summary>
        /// <param name="picture">Picture</param>
        protected virtual void DeletePictureOnFileSystem(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            string lastPart = MimeTypes.MapMimeTypeToExtension(picture.MimeType);
            string FileName = string.Format("{0}-0.{1}", picture.Id.ToString("0000000"), lastPart);
            string filePath = GetPictureLocalPath(FileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Validates input picture dimensions and prevents that the image size exceeds global max size
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        public virtual byte[] ValidatePicture(byte[] pictureBinary)
        {
            Size originalSize = this.GetPictureSize(pictureBinary);
  
            int maxSize = _mediaSettings.MaximumImageSize;
            if (originalSize.IsEmpty || (originalSize.Height <= maxSize && originalSize.Width <= maxSize))
            {
                return pictureBinary;
            }

            using (var resultStream = _imageResizerService.ResizeImage(new MemoryStream(pictureBinary), maxSize, maxSize, _mediaSettings.DefaultImageQuality))
            {
                return resultStream.GetBuffer();
            }
        }

        private string GetDefaultImageFileName(PictureType defaultPictureType = PictureType.entity)
        {
            string defaultImageFileName;
            switch (defaultPictureType)
            {
                case PictureType.avatar:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultAvatarImageName", "default-avatar.jpg");
                    break;
                case PictureType.photo:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultPhotoImageName", "default-photo.png");
                    break;
                case PictureType.marker:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.Media.DefaultMarkerId", "1");
                    break;
                case PictureType.entity:
                default:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultImageName", "default-image.jpg");
                    break;
            }

            return defaultImageFileName;
        }

        private Bitmap GausianBlur(Bitmap bmp, bool alphaEdgesOnly, Size blurSize)
        {
            var pixelY = 0;
            var pixelX = 0;

            try
            {
                // UI Stuff
                // Loop the rows of the image
                for (pixelY = 0; pixelY < bmp.Width - 1; pixelY++)
                {
                    // Loop the cols of the image
                    for (pixelX = 0; pixelX < bmp.Height - 1; pixelX++)
                    {
                        if (!alphaEdgesOnly)
                        {
                            bmp.SetPixel(pixelX, pixelY, Average(bmp, blurSize, bmp.PhysicalDimension, pixelX, pixelY));
                        }
                        else if (bmp.GetPixel(pixelX, pixelY).A != 255)
                        {
                            bmp.SetPixel(pixelX, pixelY, Average(bmp, blurSize, bmp.PhysicalDimension, pixelX, pixelY));
                        }
                    }
                }
            }
            catch (Exception ex) { }
            
            return bmp;
        }
        private Color Average(Bitmap bmp, Size size, SizeF imageSize, int pixelX, int pixelY)
        {
            var pixels = new List<Color>();
            var x  = 0; 
            var y = 0;
            try
            {
                // Find the color for each pixel and add it to a new array.
                // Remember a 5X5 area on or near the edge will ask for pixels that don't
                // exist in our image, this will filter those out.
                for (x = pixelX - (int)(size.Width / 2); x < pixelX + (int)(size.Width / 2); x++)
                {
                    for (y = pixelY - (int)(size.Height / 2); y < pixelY + (int)(size.Height / 2); y++)
                    {
                        if ((x > 0 && x < imageSize.Width) &&
                            (y > 0 && y < imageSize.Height))
                        {
                            pixels.Add(bmp.GetPixel(x, y));
                        }
                    }
                }
            }
            catch (Exception ex) { }

                // Average the A, R, G, B channels 
                // reflected in the array
            var alpha = 0;
            var red = 0;
            var green = 0;
            var blue = 0;

            foreach (var thisColor in pixels)
            {
                alpha += thisColor.A;
                red += thisColor.R;
                green += thisColor.G;
                blue += thisColor.B;
            }

            // Return the sum of the colors / the number of colors (The average)
            return Color.FromArgb(alpha / pixels.Count, 
                red / pixels.Count, 
                green / pixels.Count, 
                blue / pixels.Count);
        }
        #endregion Utilities

        #region Methods
        #region Picture
        /// <summary>
        /// Get picture SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public virtual string GetPictureSeName(string name)
        {
            return null; // SeoHelper.GetSeName(name, true, false);
        }

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public virtual string GetThumbLocalPath(Picture picture, int targetSize = 0, bool showDefaultPicture = true)
        {
            // 'GetPictureUrl' takes care of creating the thumb when not created already
            string url = this.GetPictureUrl(picture, targetSize, showDefaultPicture);

            if (url.HasValue())
            {
                var settings = this.CreateResizeSettings(targetSize);

                var cachedImage = _imageCache.GetCachedImage(picture, settings);
                if (cachedImage.Exists)
                {
                    return cachedImage.LocalPath;
                }

                if (showDefaultPicture)
                {
                    var FileName = this.GetDefaultImageFileName();
                    cachedImage = _imageCache.GetCachedImage(
                        0,
                        Path.GetFileNameWithoutExtension(FileName),
                        Path.GetExtension(FileName).TrimStart('.'),
                        settings);
                    if (cachedImage.Exists)
                    {
                        return cachedImage.LocalPath;
                    }
                }  
            }

            return string.Empty;

        }

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>Picture URL</returns>
        public virtual string GetDefaultPictureUrl(int targetSize = 0, PictureType defaultPictureType = PictureType.entity)
        {
            string defaultImageFileName = GetDefaultImageFileName(defaultPictureType);

            string filePath = string.Empty;
            if (defaultPictureType != PictureType.marker)
            {
                filePath = GetDefaultPictureLocalPath(defaultImageFileName);
            }

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
             
            if (targetSize == 0)
            {
                string url = filePath; /*_webHelper.GetSiteLocation() + */ /*"/Content/Images/" + defaultImageFileName;*/
                return url;
            }
            else
            {
                var url = this.GetProcessedImageUrl(
                    filePath, 
                    0,
                    Path.GetFileNameWithoutExtension(filePath),
                    Path.GetExtension(filePath), 
                    targetSize);

                return url;
            }
        }

        protected internal virtual string GetProcessedImageUrl(object source, long? pictureId, string seoFileName, string extension, int targetSize = 0, PictureType type = PictureType.entity)
        {   
            var cachedImage = _imageCache.GetCachedImage(
                pictureId,
                seoFileName,
                extension,
                this.CreateResizeSettings(targetSize));
            ResizeSettings resizeSettings = null;

            if (!cachedImage.Exists)
            {
                lock (s_lock)
                {
                    if (!File.Exists(cachedImage.LocalPath)) // check again
                    {
                        var buffer = source as byte[];
                        if (buffer == null)
                        {
                            if (!(source is string))
                            {
                                return string.Empty;
                            }

                            try
                            {
                                buffer = File.ReadAllBytes((string)source);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error("Error reading media file '{0}'.".FormatInvariant(source), ex);
                                return string.Empty;
                            }
                        }

                        try
                        {
                            if (targetSize == 0)
                            {
                                _imageCache.AddImageToCache(cachedImage, buffer);
                            }
                            else
                            {
                                var sourceStream = new MemoryStream(buffer);
                                switch (type)
                                {
                                    case PictureType.photo:
                                        resizeSettings = new ResizeSettings(targetSize, targetSize, FitMode.Crop, "png");
                                        break;
                                }
                                using (var resultStream = _imageResizerService.ResizeImage(sourceStream, targetSize, targetSize, _mediaSettings.DefaultImageQuality, resizeSettings))
                                {
                                    _imageCache.AddImageToCache(cachedImage, resultStream.GetBuffer());
                                    //File.WriteAllBytes(cachedImage.LocalPath, resultStream.GetBuffer());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error processing/writing media file '{0}'.".FormatInvariant(cachedImage.LocalPath), ex);
                            return string.Empty;
                        }
                    }
                }
            }

            var url = _imageCache.GetImageUrl(cachedImage.Path);
            return url;
        }

        /// <summary>
        /// Loads a picture from file
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary</returns>
        protected virtual byte[] LoadPictureFromFile(long pictureId, string mimeType)
        {
            string lastPart = MimeTypes.MapMimeTypeToExtension(mimeType);
            string FileName = string.Format("{0}-0.{1}", pictureId.ToString("0000000"), lastPart);
            var filePath = GetPictureLocalPath(FileName);
            if (!File.Exists(filePath))
            {
                return new byte[0];
            }
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>Picture binary</returns>
        public virtual byte[] LoadPictureBinary(Picture picture)
        {
            return LoadPictureBinary(picture, this.StoreInDb);
        }

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <param name="fromDb">Load from database; otherwise, from file system</param>
        /// <returns>Picture binary</returns>
        public virtual byte[] LoadPictureBinary(Picture picture, bool fromDb)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            byte[] result = null;
            if (fromDb)
            {
                result = picture.PictureBinary;
            }
            else
            {
                result = LoadPictureFromFile(picture.Id, picture.MimeType);
            }

            return result;
        }

        public virtual Size GetPictureSize(Picture picture)
        {
            byte[] pictureBinary = LoadPictureBinary(picture);
            return GetPictureSize(pictureBinary);
        }

        public virtual Size GetPictureSize(long pictureId)
        {
            if (pictureId != 0) return Size.Empty;
            var picture = GetPictureById(pictureId);
            return GetPictureSize(picture);
        }

        public Size GetPictureSize(byte[] pictureBinary)
        {
            if (pictureBinary == null || pictureBinary.Length == 0)
            {
                return new Size();
            }
            
            var stream = new MemoryStream(pictureBinary);
            
            Size size;

            try
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    size = ImageHeader.GetDimensions(reader);
                }
            }
            catch (Exception)
            {
                // something went wrong with fast image access,
                // so get original size the classic way
                using (var b = new Bitmap(stream))
                {
                    size = new Size(b.Width, b.Height);
                }
            }
            finally
            {
                stream.Dispose();
            }

            return size;
        }

        public int GetDefaultSize(string type)
        {
            var _type = type.ToEnum<PictureType>(PictureType.entity);
            switch(_type)
            {
                case PictureType.avatar:
                    return _mediaSettings.AvatarPictureSize;
                case PictureType.marker:
                    return _mediaSettings.DeviceMarkerMaxSize;
                case PictureType.photo:
                    return _mediaSettings.UserPhotoSize;
                case PictureType.entity:
                default:
                    return _mediaSettings.MaximumImageSize;
            }
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        public virtual string GetPictureUrl(
            long pictureId,
            int targetSize = 0,
            bool showDefaultPicture = true,
            PictureType defaultPictureType = PictureType.entity)
        {
            var picture = GetPictureById(pictureId);
            return GetPictureUrl(picture, targetSize, showDefaultPicture, defaultPictureType);
        }

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        public virtual string GetPictureUrl(
            Picture picture,
            int targetSize = 0,
            bool showDefaultPicture = true,
            PictureType defaultPictureType = PictureType.entity)
        {
            string url = string.Empty;
            byte[] pictureBinary = null;
            if (picture != null)
                pictureBinary = LoadPictureBinary(picture);
            if (picture == null || pictureBinary == null || pictureBinary.Length == 0)
            {
                if (showDefaultPicture)
                {
                    url = GetDefaultPictureUrl(targetSize, defaultPictureType);
                }
                return url;
            }

            if (picture.IsNew)
            {
                _imageCache.DeleteCachedImages(picture);

                // we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                picture = UpdatePicture(picture.Id,
                    pictureBinary,
                    picture.MimeType,
                    picture.SeoFileName,
                    picture.IsNew,
                    false);
            }

            url = this.GetProcessedImageUrl(
                pictureBinary,
                picture.Id,
                picture.SeoFileName,
                MimeTypes.MapMimeTypeToExtension(picture.MimeType),
                targetSize,
                defaultPictureType);

            return url;
        }

        public virtual string GetPhotoUrl(long pictureId)
        {
            var picture = GetPictureById(pictureId);
            return GetPictureUrl(picture, 0, true, PictureType.photo);
        }

        private ResizeSettings CreateResizeSettings(int targetSize)
        {
            var settings = new ResizeSettings();
            if (targetSize > 0)
            {
                settings.MaxWidth = targetSize;
                settings.MaxHeight = targetSize;
            }

            return settings;
        }

        /// <summary>
        /// Get picture local path. Used when images stored on file system (not in the database)
        /// </summary>
        /// <param name="FileName">FileName</param>
        /// <returns>Local picture path</returns>
        protected virtual string GetPictureLocalPath(string FileName)
        {
            var imagesDirectoryPath = _webHelper.MapPath("~/Media/");
            var filePath = Path.Combine(imagesDirectoryPath, FileName);
            return filePath;
        }
        protected virtual string GetDefaultPictureLocalPath(string FileName)
        {
            var dirPath = _webHelper.MapPath("~/Content/Images");
            var filePath = Path.Combine(dirPath, FileName);
            return filePath;
        }

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture</returns>
        public virtual Picture GetPictureById(long pictureId, PictureType defaultPictureType = PictureType.entity)
        {
            if (pictureId == 0)
                return null;
            
            Picture picture;

            if (defaultPictureType == PictureType.marker)
                picture = _pictureRepository.Where(p => p.SeoFileName.StartsWith("marker") && p.Id == pictureId).FirstOrDefault();
            else
                picture = _pictureRepository.GetById(pictureId);
            return picture;
        }
        #endregion Picture

        #region Markers
        protected virtual string GetMarkerPath(Marker marker)
        {
            var markersPath = _mediaSettings.MarkersPath;
            if (!markersPath.EndsWith("/")) { markersPath += "/"; }
            string lastPart = MimeTypes.MapMimeTypeToExtension(marker.MimeType);
            string fileName = string.Format("{0}-0.{1}", marker.Id.ToString("0000000"), lastPart);
            var url = markersPath + fileName;
            return url;
        }
        protected virtual string GetMarkerLocalPath(Marker marker)
        {
            var dirPath = _webHelper.MapPath("~/Media/Markers");
            string lastPart = MimeTypes.MapMimeTypeToExtension(marker.MimeType);
            string fileName = string.Format("{0}-0.{1}", marker.Id.ToString("0000000"), lastPart);
            var filePath = Path.Combine(dirPath, fileName);
            return filePath;
        }
        protected virtual byte[] LoadMarkerFromFile(Marker marker)
        {
            var filePath = GetMarkerLocalPath(marker);
            if (!File.Exists(filePath))
            {
                return new byte[0];
            }
            return File.ReadAllBytes(filePath);
        }
        protected virtual byte[] LoadMarkerFromFile(long markerId)
        {
            var marker = GetMarkerById(markerId);
            return LoadMarkerFromFile(marker);
        }
        public virtual Marker GetDefaultMarker()
        {
            return GetMarkerById(_mediaSettings.DefaultMarkerId);
        }
        public virtual Marker GetMarkerById(long markerId)
        {
            if (markerId == 0)
                return GetDefaultMarker();

            var marker = _markerRepository.GetById(markerId);
            return marker;
        }
        public virtual Size GetMarkerSize(long markerId)
        {
            if (markerId == 0)
                return Size.Empty;
            
            var marker = _markerRepository.GetById(markerId);
            return new Size(marker.Width, marker.Height);
        }
        public virtual IList<Marker> GetMarkers()
        {
            var markers = _markerRepository.Table
                .Where(marker => marker.ShadowMarkerId != null)
                .OrderBy(marker => marker.Id)
                .ToList();
            return markers;
        }
        public virtual IList<IndexedMarker> GetSystemMarkers()
        {
            return _cacheManager.Get(DEVICE_ICONS_ALL_KEY, () =>
            {
                var markerList = new List<IndexedMarker>();
                var markers = _pictureRepository.Table
                    .Where(pic => pic.IsSystem && pic.SeoFileName.StartsWith("marker"))
                    .OrderBy(pic => pic.Id)
                    .ToList();

                markerList.Add(new IndexedMarker() { Index = 1, Id = 0, Url = "" });
                var i = 2;
                foreach (var marker in markers)
                {
                    markerList.Add(new IndexedMarker() 
                    { 
                        Index = i, 
                        Id = marker.Id, 
                        Url = GetPictureUrl(marker.Id, 0, true, PictureType.marker).ToLower(),
                        Size = GetPictureSize(GetPictureById(marker.Id))
                    });
                    i++;
                }
                return markerList;
            }, _commonSettings.DeviceUserCacheTime);
        }
        public virtual string GetDefaultMarkerUrl()
        {
            var markerId = _mediaSettings.DefaultMarkerId;
            return GetMarkerUrl(markerId);
        }
        public virtual string GetMarkerUrl(long markerId)
        {
            var marker = GetMarkerById(markerId);
            return GetMarkerUrl(marker, 0, true);
        }
        public virtual string GetMarkerUrl(Marker marker, int targetSize = 0, bool showDefaultPicture = true)
        {
            var url = GetMarkerPath(marker);
            if (!this.StoreInDb && File.Exists(GetMarkerLocalPath(marker)))
            {
                return url; // Marker
            }
            else
            {
                byte[] pictureBinary = null;
                if (marker != null)
                    pictureBinary = LoadMarkerBinary(marker);
                if (marker == null || pictureBinary == null || pictureBinary.Length == 0)
                {
                    if (showDefaultPicture)
                    {
                        url = GetMarkerPath(GetDefaultMarker());
                    }
                    return url;
                }

                if (marker.IsNew)
                {
                    _imageCache.DeleteCachedMarker(marker);

                    // we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                    marker = UpdateMarker(marker.Id,
                        pictureBinary,
                        marker.MimeType,
                        marker.FileName,
                        false,
                        false);
                }

                this.SaveMarkerInFile(marker.Id, pictureBinary, marker.MimeType);
                return url;
            }
        }

        public virtual Marker GetShadowMarker(long markerId)
        {
            var marker = _markerRepository.GetById(markerId);
            if (marker == null || markerId == 0) return null;

            return _markerRepository.GetById(marker.ShadowMarkerId);
        }

        public virtual byte[] LoadMarkerBinary(Marker marker)
        {
            return LoadMarkerBinary(marker, this.StoreInDb);
        }
        public virtual byte[] LoadMarkerBinary(Marker marker, bool fromDb)
        {
            if (marker == null)
                throw new ArgumentNullException("marker");

            byte[] result = null;

            if (fromDb)
            {
                result = marker.MarkerBinary;
            }
            else
            {
                result = LoadMarkerFromFile(marker);
            }

            return result;
        }

        public Marker InsertMarker(byte[] markerBinary, string mimeType, string fileName, bool isNew, long? userId = null, bool validateBinary = true)
        {
            mimeType = mimeType.EmptyNull();
            mimeType = mimeType.Truncate(20);

            fileName = fileName.Truncate(100);

            if (validateBinary)
            {
                markerBinary = ValidatePicture(markerBinary);
            }
            var size = GetPictureSize(markerBinary);
            var marker = _markerRepository.Create();
            marker.MarkerBinary = this.StoreInDb ? markerBinary : new byte[0];
            marker.MimeType = mimeType;
            marker.FileName = fileName;
            marker.IsNew = isNew;
            marker.Width = size.Width;
            marker.Height = size.Height;
            marker.UserId = userId;

            _markerRepository.Insert(marker);

            if (!this.StoreInDb)
            {
                SaveMarkerInFile(marker.Id, markerBinary, mimeType);
            }

            //event notification
            _eventPublisher.EntityInserted(marker);

            return marker;
        }
        public Marker InsertMarker(Marker marker)
        {
            var newMarker = InsertMarker(marker.MarkerBinary, marker.MimeType, marker.FileName, marker.IsNew, marker.UserId, false);
            newMarker.Width = marker.Width;
            newMarker.Height = marker.Height;
            newMarker.IsRetina = marker.IsRetina;
            newMarker.ShadowMarkerId = marker.ShadowMarkerId != null ? marker.ShadowMarkerId : 0;
            newMarker.AnchorX = marker.AnchorX;
            newMarker.AnchorY = marker.AnchorY;
            UpdateMarker(newMarker);
            return newMarker;
        }
        public virtual Marker UpdateMarker(Marker marker)
        {
            _markerRepository.Update(marker);
            //event notification
            _eventPublisher.EntityUpdated(marker);

            return marker;
        }
        public virtual Marker UpdateMarker(long markerId, byte[] markerBinary, string mimeType, string fileName, bool isNew, bool isSystem = false, bool isRetina = false, bool validateBinary = true)
        {
            if (markerId == 0)
                return null;

            mimeType = mimeType.EmptyNull().Truncate(20);
            fileName = fileName.Truncate(100);

            if (validateBinary)
            {
                markerBinary = ValidatePicture(markerBinary);
            }

            var marker = GetMarkerById(markerId);
            if (marker == null)
                return null;

            //delete old thumbs if a picture has been changed
            if (fileName != marker.FileName)
            {
                _imageCache.DeleteCachedMarker(marker);
            }

            marker.MarkerBinary = (this.StoreInDb ? markerBinary : new byte[0]);
            marker.MimeType = mimeType;
            marker.FileName = fileName;
            marker.IsNew = isNew;

            _markerRepository.Update(marker);

            if (!this.StoreInDb)
            {
                SavePictureInFile(marker.Id, markerBinary, mimeType);
            }

            //event notification
            _eventPublisher.EntityUpdated(marker);

            return marker;
        }
        protected virtual void SaveMarkerInFile(long markerId, byte[] markerBinary, string mimeType)
        {
            var marker = GetMarkerById(markerId);
            File.WriteAllBytes(GetMarkerLocalPath(marker), markerBinary);
        }
        public virtual void DeleteMarkers(long userId)
        {
            if (userId == 0)
                return;

            var markers = _markerRepository.Table
                .Where(marker => marker.IsNew && marker.UserId == userId)
                .ToList();
            foreach(var marker in markers)
            {
                _markerRepository.Delete(marker);
                var filePath = GetMarkerLocalPath(marker);
                if (!filePath.IsNullOrEmpty())
                {
                    File.Delete(filePath);
                }
            }
        }
        public virtual void DeleteMarker(Marker marker)
        {
            if (marker == null)
                return;

            if (marker.ShadowMarkerId != 0)
            {
                var query = _markerRepository.Where(x => x.ShadowMarkerId == marker.ShadowMarkerId);
                if (query.Count() == 1)
                {
                    var shadow = _markerRepository.GetById(marker.ShadowMarkerId);
                    _markerRepository.Delete(shadow);
                }
            }
            _markerRepository.Delete(marker);

            //event notification
            _eventPublisher.EntityDeleted(marker);
        }
        #endregion Markers

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="picture">Picture</param>
        public virtual void DeletePicture(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            //delete thumbs
            _imageCache.DeleteCachedImages(picture);

            //delete from file system
            if (!this.StoreInDb)
            {
                DeletePictureOnFileSystem(picture);
            }

            //delete from database
            _pictureRepository.Delete(picture);

            //event notification
            _eventPublisher.EntityDeleted(picture);
        }

        /// <summary>
        /// Gets a collection of pictures
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        public virtual IPagedList<Picture> GetPictures(int pageIndex, int pageSize)
        {
            var query = from p in _pictureRepository.Table
                        orderby p.Id descending
                        select p;
            var pics = new PagedList<Picture>(query, pageIndex, pageSize);
            return pics;
        }

        ///// <summary>
        ///// Gets pictures by product identifier
        ///// </summary>
        ///// <param name="productId">Product identifier</param>
        ///// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        ///// <returns>Pictures</returns>
        //public virtual IList<Picture> GetPicturesByProductId(int productId, int recordsToReturn = 0)
        //{
        //    if (productId == 0)
        //        return new List<Picture>();


        //    var query = from p in _pictureRepository.Table
        //                join pp in _productPictureRepository.Table on p.Id equals pp.PictureId
        //                orderby pp.DisplayOrder
        //                where pp.ProductId == productId
        //                select p;

        //    if (recordsToReturn > 0)
        //        query = query.Take(recordsToReturn);

        //    var pics = query.ToList();
        //    return pics;
        //}

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFileName">The SEO FileName</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>Picture</returns>
        public virtual Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFileName, bool isNew, bool validateBinary = true)
        {
			mimeType = mimeType.EmptyNull();
			mimeType = mimeType.Truncate(20);

			seoFileName = seoFileName.Truncate(100);

            if (validateBinary)
            {
                pictureBinary = ValidatePicture(pictureBinary);
            }

            var picture = _pictureRepository.Create();
            picture.PictureBinary = this.StoreInDb ? pictureBinary : new byte[0];
            picture.MimeType = mimeType;
            picture.SeoFileName = seoFileName;
            picture.IsNew = isNew;

            _pictureRepository.Insert(picture);

            if (!this.StoreInDb)
            {
                SavePictureInFile(picture.Id, pictureBinary, mimeType);
            }

            //event notification
            _eventPublisher.EntityInserted(picture);

            return picture;
        }

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFileName">The SEO FileName</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>Picture</returns>
        public virtual Picture UpdatePicture(long pictureId, byte[] pictureBinary, string mimeType, string seoFileName, bool isNew, bool validateBinary = true)
        {
            mimeType = mimeType.EmptyNull().Truncate(20);
			seoFileName = seoFileName.Truncate(100);

            if (validateBinary)
            {
                pictureBinary = ValidatePicture(pictureBinary);
            }

            var picture = GetPictureById(pictureId);
            if (picture == null)
                return null;

            //delete old thumbs if a picture has been changed
            if (seoFileName != picture.SeoFileName)
            {
                _imageCache.DeleteCachedImages(picture);
            }

            picture.PictureBinary = (this.StoreInDb ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            picture.SeoFileName = seoFileName;
            picture.IsNew = isNew;

            _pictureRepository.Update(picture);

            if (!this.StoreInDb)
            {
                SavePictureInFile(picture.Id, pictureBinary, mimeType);
            }

            //event notification
            _eventPublisher.EntityUpdated(picture);

            return picture;
        }
        public virtual Picture UpdatePicture(Picture picture)
        {
            if (picture == null) return null;

            _pictureRepository.Update(picture);
            _eventPublisher.EntityUpdated(picture);

            return picture;
        }

        /// <summary>
        /// Updates a SEO FileName of a picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="seoFileName">The SEO FileName</param>
        /// <returns>Picture</returns>
        public virtual Picture SetSeoFileName(int pictureId, string seoFileName)
        {
            var picture = GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            //update if it has been changed
            if (seoFileName != picture.SeoFileName)
            {
                //update picture
                picture = UpdatePicture(picture.Id, LoadPictureBinary(picture), picture.MimeType, seoFileName, true, false);
            }
            return picture;
        }

        public virtual string CreateShadow(long pictureId)
        {
            var marker = GetSystemMarkers().FirstOrDefault(x => x.Id == pictureId);

            string sourceFileName = @"E:\Projects\Specter.GPS.Server\Web\Specter.Web\Content\Images\marker-blue.png"; //CommonHelper.MapPath(marker.Url);
            string targetFileName = @"E:\Projects\Specter.GPS.Server\Web\Specter.Web\Content\Images\marker-blue-shadow.png";
            using (var source = (Bitmap)Bitmap.FromFile(sourceFileName))
            {
                using (Bitmap target = new Bitmap(source.Width * 2, source.Height, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(target))
                    {
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddPolygon(new PointF[] { new Point2D(0, 0), new Point2D(source.Width, 0), new Point2D(0, source.Height) });
                        var matrix = new Matrix();
                        matrix.Reset();
                        matrix.Shear(-0.25f, -0.25f, MatrixOrder.Append);
                        matrix.Translate(0.5f * source.Width, 0.125f * source.Height, MatrixOrder.Append);
                        gp.Transform(matrix);
                        g.DrawImage(source, gp.PathPoints);
                    }
                    var grayscale = new Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb);
                    for (var x = 0; x < target.Width; x++)
                    {
                        for (var y = 0; y < target.Height; y++)
                        {
                            var pixel = target.GetPixel(x, y);
                            if (pixel.A != 0)
                            {
                                pixel = Color.FromArgb(255 / 3, Color.Black);
                            }
                            grayscale.SetPixel(x, y, pixel);
                        }
                    }
                    for (var x = 0; x < source.Width / 2; x++)
                    {
                        for (var y = source.Height - 1; y >= 0; y--)
                        {
                            var pixel = source.GetPixel(x, y);
                            if (pixel.A != 0)
                            {
                                break;
                            }
                            else
                            {
                                grayscale.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                            }
                        }
                    }
                    grayscale = GausianBlur(grayscale, false, new Size(6, 6));
                    grayscale.Save(targetFileName, ImageFormat.Png);
                }
            }
            return null;
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        public virtual bool StoreInDb
        {
            get
            {
                return _settingService.GetSettingByKey<bool>("Media.Images.StoreInDB", true);
            }
            set
            {
                //check whether it's a new value
                if (this.StoreInDb != value)
                {
                    //save the new setting value
                    _settingService.SetSetting<bool>("Media.Images.StoreInDB", value);

                    //update all picture objects
                    var pictures = this.GetPictures(0, int.MaxValue);
                    foreach (var picture in pictures)
                    {
                        var pictureBinary = LoadPictureBinary(picture, !value);

                        //delete from file system
                        if (value)
                            DeletePictureOnFileSystem(picture);

                        //just update a picture (all required logic is in UpdatePicture method)
                        UpdatePicture(picture.Id,
                                      pictureBinary,
                                      picture.MimeType,
                                      picture.SeoFileName,
                                      true,
                                      false);
                        //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                    }
                }
            }
        }
        #endregion Properties
    }
}
