using System;
﻿using System.Collections.Generic;
using Codestetic.Web.Core.Configuration;
﻿using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.Mvc;
using Kendo.Mvc;

using FluentValidation.Attributes;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Framework.Localization;
using Codestetic.Web.Areas.Admin.Validators.ContentSlider;

namespace Codestetic.Web.Areas.Admin.Models.ContentSlider
{
    //[Validator(typeof(ContentSliderValidator))]
    public class ContentSliderSettingsModel : EntityModelBase
    {
        public ContentSliderSettingsModel()
        {
            Slides = new List<ContentSliderSlideModel>();
			AvailableStores = new List<SelectListItem>();
        }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Active")]
        public bool Active { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.SliderHeight")]
        public string ContentSliderHeight { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Background")]
        public int BackgroundPictureId { get; set; }

        public string BackgroundPictureUrl { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.AutoPlay")]
        public bool AutoPlay { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.AutoPlayDelay")]
        public int AutoPlayDelay { get; set; }

        public IList<ContentSliderSlideModel> Slides { get; set; }
		public IList<SelectListItem> AvailableStores { get; set; }

		[ResourceDisplayName("Admin.Common.Store.SearchFor")]
		public int SearchStoreId { get; set; }
    }

    [Validator(typeof(ContentSliderSlideValidator))]
    public class ContentSliderSlideModel : EntityModelBase
    {
		public int SlideIndex { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Text")]
        [AllowHtml]
        public string Text { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Picture")]
        public int PictureId { get; set; }

        public string PictureUrl { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Language")]
        public string LanguageName { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Language")]
        public string LanguageCulture { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Published")]
        public bool Published { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Button1")]
        public ContentSliderButtonModel Button1 { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Button2")]
        public ContentSliderButtonModel Button2 { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Slide.Button3")]
        public ContentSliderButtonModel Button3 { get; set; }
    }

    [Validator(typeof(ContentSliderButtonValidator))]
    public class ContentSliderButtonModel : EntityModelBase
    {
        [Display(Description = "Admin.Configuration.ContentSlider.Button.Text.Hint")]
        [ResourceDisplayName("Admin.Configuration.ContentSlider.Button.Text")]
        public string Text { get; set; }

        [ResourceDisplayName("Admin.Configuration.ContentSlider.Button.Type")]
        [Display(Description = "Admin.Configuration.ContentSlider.Button.Type.Hint")]
        [UIHint("ButtonType")]
        public string Type { get; set; }

        [Display(Description = "Admin.Configuration.ContentSlider.Button.Url.Hint")]
        [ResourceDisplayName("Admin.Configuration.ContentSlider.Button.Url")]
        public string Url { get; set; }

        [Display(Description = "Admin.Configuration.ContentSlider.Button.Published.Hint")]
        [ResourceDisplayName("Admin.Configuration.ContentSlider.Button.Published")]
        public bool Published { get; set; }
    }
}