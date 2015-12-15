using Codestetic.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Models.Setting
{
    public class TrackModel : EntityModelBase
    {
        #region Constructors
        public TrackModel() { }
        #endregion Constructors

        #region Properties
        public bool Enable { get; set; }
        public int Length { get; set; }
        public int StrokeWidth { get; set; }
        public string StrokeColor { get; set; }
        #endregion Properties
    }
}