using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages;

namespace Codestetic.Web.Framework.UI
{
    public enum EnumEffect
    {
        blind,
        bounce,
        clip,
        drop,
        explode,
        fade,
        fold,
        highlight,
        puff,
        pulsate,
        scale,
        shake,
        size,
        slide,
        transfer,
    }

    public class Window : Component
    {
        public Window()
        {
            this.Modal = true;
            this.Resizable = false;
            this.Draggable = false;
            this.Visible = true;
            this.BackDrop = true;
            this.ShowClose = true;
            this.CloseOnEscapePress = true;
            this.Buttons = new List<Button>();
        }

        public override bool NameIsRequired
        {
            get { return true; }
        }

        public string Initiator { get; set; }
        public string Title { get; set; }
        public HelperResult Content { get; set; }
        public string ContentUrl { get; set; }
        public bool Modal { get; set; }
        public bool Resizable { get; set; }
        public bool Draggable { get; set; }
        public VisualEffect ShowAnimation { get; set; }
        public VisualEffect HideAnimation { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public int? MinWidth { get; set; }
        public int? MinHeight { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool CloseOnEscapePress { get; set; }
        public bool Visible { get; set; }
        public List<Button> Buttons { get; set; }

        public bool BackDrop { get; set; }
        public bool ShowClose { get; set; }
        public HelperResult FooterContent { get; set; }
    }
}
