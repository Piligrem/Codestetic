using System.Collections.Generic;
using System.Web.Mvc;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Areas.Admin.Models.Settings
{
	public class MediaSettingsModel : ModelBase
    {
        public MediaSettingsModel()
        {
            this.AvailablePictureZoomTypes = new List<SelectListItem>();
        }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.DeviceUserPhotoSize")]
        public int DeviceUserPhotoSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.DeviceUserPhotoBigSize")]
        public int DeviceUserPhotoBigSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.DeviceUserMarkerMaxSize")]
        public int DeviceUserMarkerMaxSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.ProductThumbPictureSize")]
        public int ProductThumbPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.ProductDetailsPictureSize")]
        public int ProductDetailsPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.ProductThumbPictureSizeOnProductDetailsPage")]
        public int ProductThumbPictureSizeOnProductDetailsPage { get; set; }

		[ResourceDisplayName("Admin.Configuration.Settings.Media.AssociatedProductPictureSize")]
        public int AssociatedProductPictureSize { get; set; }

		[ResourceDisplayName("Admin.Configuration.Settings.Media.BundledProductPictureSize")]
		public int BundledProductPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.ManufacturerThumbPictureSize")]
        public int ManufacturerThumbPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }

		[ResourceDisplayName("Admin.Configuration.Settings.Media.CartThumbBundleItemPictureSize")]
		public int CartThumbBundleItemPictureSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }
        
        [ResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
        public int MaximumImageSize { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled")]
        public bool DefaultPictureZoomEnabled { get; set; }

        [ResourceDisplayName("Admin.Configuration.Settings.Media.PictureZoomType")]
        public string PictureZoomType { get; set; }

        public List<SelectListItem> AvailablePictureZoomTypes { get; set; }

    }
}