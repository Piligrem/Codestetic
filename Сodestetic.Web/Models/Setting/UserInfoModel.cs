using Codestetic.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Models.Setting
{
    public class UserInfoModel : EntityModelBase
    {
        #region Constructors
        public UserInfoModel() { }
        #endregion Constructors

        #region Properties
        public string Name { get; set; }
        public long PhotoId { get; set; }
        public long MarkerId { get; set; }
        #endregion Properties
    }
}