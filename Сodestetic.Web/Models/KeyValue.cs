using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Codestetic.Web.Models
{
    public class PictureRequest
    {
        public long pictureId { get; set; }
        public bool OriginalSize { get; set; }
    }
}