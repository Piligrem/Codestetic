using System.Drawing;

namespace Specter.Web.Core.Domain.Media
{
    public class IndexedMarker
    {
        public int Index { get; set; }
        public long Id { get; set; }
        public string Url { get; set; }
        public Size Size { get; set; }
    }
}
