using Codestetic.Web.Framework.Mvc;
using System.Drawing;

namespace Codestetic.Web.Models.Media
{
    public class MarkerModel : EntityModelBase
    {
        public MarkerModel()
        {
            Size = Size.Empty;
            Anchor = AnchorModel.Empty;
            Url = string.Empty;
        }
        public string Name { get; set; }
        public string Url { get; set; }
        public Size Size { get; set; }
        public AnchorModel Anchor { get; set; }
        public bool IsSystem { get; set; }
        public bool IsRetina { get; set; }
        public MarkerModel ShadowMarker { get; set; }
    }
}