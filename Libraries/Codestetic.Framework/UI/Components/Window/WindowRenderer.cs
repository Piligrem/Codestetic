using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.Mvc;
using Codestetic.Web.Core;

namespace Codestetic.Web.Framework.UI
{
   
    public class WindowRenderer : ComponentRenderer<Window>
    {
        protected override void WriteHtmlCore(HtmlTextWriter writer)
        {
            var win = base.Component;

            win.HtmlAttributes.AppendCssClass("modal");
            win.HtmlAttributes["role"] = "dialog";
            win.HtmlAttributes["aria-labelledby"] = win.Id + "Label";
            win.HtmlAttributes["data-keyboard"] = win.CloseOnEscapePress.ToString().ToLower();
            if (win.ContentUrl.HasValue())
            {
                win.HtmlAttributes["data-remote"] = win.ContentUrl;
            }

            writer.AddAttributes(win.HtmlAttributes);
            writer.RenderBeginTag("div"); // root div

            // HEADER
            //if (win.ShowClose && win.Title.HasValue())
            //{
            //    this.RenderHeader(writer);
            //}

            // BODY
            this.RenderBody(writer);

            // Script
            this.RenderScript(writer);

            // FOOTER
            //if (win.FooterContent != null)
            //{
            //    this.RenderFooter(writer);
            //}
            writer.RenderEndTag(); // div.modal
        }

        protected virtual void RenderHeader(HtmlTextWriter writer)
        {
            var win = base.Component;

            writer.AddAttribute("class", "modal-header");
            writer.RenderBeginTag("div");

            if (win.ShowClose)
            {
                writer.Write("<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>×</button>");
            }

            if (win.Title.HasValue())
            {
                writer.Write("<h3 id='{0}'>{1}</h3>".FormatCurrent(win.Id + "Label", win.Title));
            }

            writer.RenderEndTag(); // div.modal-header
        }
        protected virtual void RenderBody(HtmlTextWriter writer)
        {
            var win = base.Component;

            writer.AddAttribute("class", "modal-body-admin");

            writer.RenderBeginTag("div");

            if (win.ContentUrl.IsEmpty() && win.Content != null)
            {
                win.Content.WriteTo(writer);
            }
            writer.RenderEndTag(); // div.modal-body
        }
        protected virtual void RenderFooter(HtmlTextWriter writer)
        {
            var win = base.Component;

            writer.AddAttribute("class", "modal-footer");
            writer.RenderBeginTag("div");

            //writer.WriteLine(win.FooterContent.ToString());
            win.FooterContent.WriteTo(writer);

            writer.RenderEndTag(); // div.modal-body
        }
        protected virtual void RenderScript(HtmlTextWriter writer)
        {
            var win = base.Component;
            var script = new StringBuilder();
            script.Append("\r\n<script>");
            script.Append("$(function(){");
            script.Append("$('#" + win.Id + "').dialog({");
            
            var buttons = new List<string>();
            foreach (var button in win.Buttons)
            {
                object command = "{}";
                if (button.Action is string && ((string)button.Action).HasValue())
                {
                    command = "function(){" + (string)button.Action + ";";
                    command += "$('#" + win.Id + "').find('form').submit();";
                    command += "$(this).dialog('close');}";
                }
                else if (button.Action is EnumActions)
                {
                    switch ((EnumActions)button.Action)
                    {
                        case EnumActions.Submit:
                            command = "function(){$('#" + win.Id + "').find('form').submit();";
                            command += "$(this).dialog('close');}";
                            break;
                        case EnumActions.Close:
                            command = "function(){$(this).dialog('close');}";
                            break;
                        case EnumActions.Function:
                            command = "";
                            command += "";
                            command += "$(this).dialog('close');}";
                            break;
                    }
                }
                buttons.Add(string.Format("'{0}':{1}", button.Title, (string)command));
            }

            var options = new List<string>();
            options.Add(string.Format("autoOpen:{0}", win.Visible.ToString().ToLower()));
            options.Add(string.Format("title:'{0}'", win.Title));
            options.Add(string.Format("modal:{0}", win.Modal.ToString().ToLower()));
            options.Add(string.Format("resizable:{0}", win.Resizable.ToString().ToLower()));
            if (win.MaxWidth != null)
                options.Add(string.Format("maxWidth:{0}", win.MaxWidth.Value));
            if (win.MaxHeight != null)
                options.Add(string.Format("maxHeight:{0}", win.MaxHeight.Value));
            if (win.MinWidth != null)
                options.Add(string.Format("minWidth:{0}", win.MinWidth.Value));
            if (win.MinHeight != null)
                options.Add(string.Format("minHeight:{0}", win.MinHeight.Value));
            if (win.Width != null)
                options.Add(string.Format("width:{0}", win.Width.Value));
            if (win.Height != null)
                options.Add(string.Format("height:{0}", win.Height.Value));
            options.Add(string.Format("draggable:{0}", win.Draggable.ToString().ToLower()));
            options.Add(string.Format("closeOnEscape:{0}", win.CloseOnEscapePress.ToString().ToLower()));
            if (win.ShowAnimation != null)
                options.Add("show:{effect:'" + win.ShowAnimation.Effect + "',delay:" + win.ShowAnimation.Delay + "}");
            if (win.HideAnimation != null)
                options.Add("hide:{effect:'" + win.HideAnimation.Effect + "',delay:" + win.HideAnimation.Delay + "}");
            if (win.Buttons.Count() > 0)
            {
                options.Add("buttons:{" + string.Join(",", buttons) + "}");
            }
            script.Append(string.Join(",", options));
            script.Append("});");
            script.Append("});");
            if (win.Initiator.HasValue())
            {
                var initiator = new List<string>();
                initiator.Add("$('#" + win.Initiator + "').click(function(e){");
                initiator.Add("e.preventDefault();");
                initiator.Add("$('#" + win.Id + "').dialog('open');");
                initiator.Add("})");
                script.Append(string.Concat(initiator));
            }
            script.Append("</script>");

            writer.WriteLine(script.ToString());
        }
    }
}
