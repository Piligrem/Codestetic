﻿using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.Common
{
    public partial class LanguageModel : EntityModelBase
    {
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string SeoCode { get; set; }
        public string NativeName { get; set; }
        public string ReturnUrl { get; set; }
        public string FlagImageFileName { get; set; }
    }
}