using System.Collections.Generic;
using System.Drawing;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Core;

namespace Codestetic.Web.Services.Media
{
    /// <summary>
    /// Picture service interface
    /// </summary>
    public partial interface IPictureService
    {
        /// <summary>
        /// Validates input picture dimensions and prevents that the image size exceeds global max size
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        byte[] ValidatePicture(byte[] pictureBinary);
        
        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>Picture binary</returns>
        byte[] LoadPictureBinary(Picture picture);

        /// <summary>
        /// Get picture SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        string GetPictureSeName(string name);

        /// <summary>
        /// Gets the size of a picture
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        Size GetPictureSize(Picture picture);
        Size GetPictureSize(byte[] pictureBinary);
        Size GetPictureSize(long pictureId);
        int GetDefaultSize(string type);

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <returns>Picture URL</returns>
        string GetDefaultPictureUrl(int targetSize = 0,
            PictureType defaultPictureType = PictureType.entity);

        #region Markers
        Marker GetMarkerById(long iconId);
        Size GetMarkerSize(long iconId);
        Marker GetDefaultMarker();
        string GetDefaultMarkerUrl();
        IList<IndexedMarker> GetSystemMarkers();
        IList<Marker> GetMarkers();
        string GetMarkerUrl(long iconId);
        Marker GetShadowMarker(long iconId);
        Marker InsertMarker(byte[] iconBinary, string mimeType, string fileName, bool isNew, long? userId = null, bool validateBinary = true);
        Marker InsertMarker(Marker icon);
        Marker UpdateMarker(Marker icon);
        Marker UpdateMarker(long iconId, byte[] iconBinary, string mimeType, string fileName, bool isNew, bool isSystem = false, bool isRetina = false, bool validateBinary = true);
        void DeleteMarkers(long userId);
        void DeleteMarker(Marker icon);
        #endregion Markers

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        string GetPictureUrl(long pictureId,
            int targetSize = 0,
            bool showDefaultPicture = true,
            PictureType defaultPictureType = PictureType.entity);

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="storeLocation">Store location URL; null to use determine the current store location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        string GetPictureUrl(Picture picture,
            int targetSize = 0,
            bool showDefaultPicture = true,
            PictureType defaultPictureType = PictureType.entity);

        string GetPhotoUrl(long pictureId);

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture should be shown</param>
        /// <returns></returns>
        string GetThumbLocalPath(Picture picture, int targetSize = 0, bool showDefaultPicture = true);

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture</returns>
        Picture GetPictureById(long pictureId, PictureType defaultPictureType = PictureType.entity);

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="picture">Picture</param>
        void DeletePicture(Picture picture);

        /// <summary>
        /// Gets a collection of pictures
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        IPagedList<Picture> GetPictures(int pageIndex, int pageSize);

        string CreateShadow(long pictureId);

        ///// <summary>
        ///// Gets pictures by product identifier
        ///// </summary>
        ///// <param name="productId">Product identifier</param>
        ///// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        ///// <returns>Pictures</returns>
        //IList<Picture> GetPicturesByProductId(long productId, int recordsToReturn = 0);

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFileName">The SEO FileName</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>Picture</returns>
        Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFileName, bool isNew, bool validateBinary = true);

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
        Picture UpdatePicture(long pictureId, byte[] pictureBinary, string mimeType, string seoFileName, bool isNew, bool validateBinary = true);
        Picture UpdatePicture(Picture picture);

        ///// <summary>
        ///// Updates a SEO FileName of a picture
        ///// </summary>
        ///// <param name="pictureId">The picture identifier</param>
        ///// <param name="seoFileName">The SEO FileName</param>
        ///// <returns>Picture</returns>
        //Picture SetSeoFileName(long pictureId, string seoFileName);

        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        bool StoreInDb { get; set; }
    }
}
