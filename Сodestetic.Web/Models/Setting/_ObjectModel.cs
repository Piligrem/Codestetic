using Codestetic.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Models.Setting
{
    public class _ObjectModel : EntityModelBase
    {
        #region Constructors
        public _ObjectModel() { }
        #endregion Constructors

        #region Properties
        public UserInfoModel UserInfo { get; set; }
        public ToleranceModel Tolerance { get; set; }
        public TrackModel Track { get; set; }
        #endregion Properties

        public class UserModel
        {
            public object Photo { get; set; }
            public object Marker { get; set; }
        }
    }
}