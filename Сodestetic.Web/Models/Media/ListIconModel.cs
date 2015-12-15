using Codestetic.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Drawing;

namespace Codestetic.Web.Models.Media
{
    public class ListMarkerModel : ModelBase
    {
        public ListMarkerModel()
        {
            Markers = new List<MarkerModel>();
        }
        public MarkerModel CurrentMarker { get; set; }
        public List<MarkerModel> Markers { get; set; }
        public Size MarkerMaxSize { get; set; }
    }
}