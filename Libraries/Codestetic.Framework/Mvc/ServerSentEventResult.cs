using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Codestetic.Web.Framework.Mvc
{
    public class ServerSentEventResult : ActionResult
    {
        public delegate string GetContent();
        public GetContent Content { get; set; }
        public long Version { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (this.Content != null)
            {
                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = "text/event-stream"; response.BufferOutput = false; response.Charset = null;
                string[] newStrings = context.HttpContext.Request.Headers.GetValues("Last-Event-ID");
                if (newStrings == null || newStrings[0] != this.Version.ToString())
                {
                    try
                    {
                        //response.Write("retry:250\n");
                        //response.Write(string.Format("id:{0}\n", this.Version));
                        //response.Write(string.Format("data:{0}\n\n", this.Content()));
                        //response.End();
                        var sb = new StringBuilder();
                        sb.Append("retry: 1\n");
                        sb.AppendFormat("id: {0}\n", this.Version);
                        sb.AppendFormat("id: {0}\n\n", this.Content());
                        response.ContentType = "text/event-stream";
                        response.Write(sb.ToString());
                        response.Flush();
                    }
                    catch (HttpException ex) { Debug.WriteLine(ex.Message); }
                }
                else
                {
                    response.Write(String.Empty);
                }
            }
        }
    }
}
