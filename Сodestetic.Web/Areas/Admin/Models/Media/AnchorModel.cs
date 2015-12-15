using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Areas.Admin.Models.Media
{
    public class AnchorModel
    {
        #region Constructors
        public AnchorModel() { }
        public AnchorModel(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        #endregion Constructors

        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        #endregion Properties

        #region Methods
        public static AnchorModel Empty
        {
            get { return new AnchorModel(0, 0); }
        }
        #endregion Methods
    }
}