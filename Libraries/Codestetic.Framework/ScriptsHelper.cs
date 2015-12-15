using System.Text;

namespace Codestetic.Web.Framework
{
    public static class ScriptsHelper
    {
        public static string FileUpload()
        {
            var script = new StringBuilder();
            script.Append("<script type='text/javascript'>");
                script.Append("$(function(){");
                    script.Append("var el=$('#photo'),");
                        script.Append("img=el.find('img'),");
                        script.Append("elPhoto=el.find('.photo-user'),");
                        script.Append("elRemove=el.find('.remove');");
                    script.Append("el.fileupload({");
                        script.Append("url:'/Media/AsyncUpload',");
                        script.Append("dataType:'json',");
            //acceptFileTypes: /^image\/(gif|jpeg|jpg|png)$/,
                        script.Append(@"acceptFileTypes:/(\.|\/)(gif|jpe?g|png)$/i,");
                        script.Append("done:function(e,data){");
                            script.Append("var result=data.result;");
                            script.Append("if(result.success){");
                                script.Append("img.attr('src',result.imageUrl);");
                                script.Append("elPhoto.val(result.pictureId);");
                                script.Append("elRemove.show();");
                            script.Append("}");
                        script.Append("},");
                        script.Append("error:function(jqXHR,textStatus,errorThrown){");
                            script.Append("if(errorThrown=='abort'){}");
                        script.Append("}");
                    script.Append("});");
                script.Append("});");
            script.Append("</script>");
            return script.ToString();
        }

        public static string Spinner()
        {
            var script = new StringBuilder();
            script.Append(";(function($){");
                script.Append("$('.ui-spinner .btn:first-of-type').on('click',function(){");
                    script.Append("$('.ui-spinner input').val(parseInt($('.ui-spinner input').val(),10)+1);");
                script.Append("});");
                script.Append("$('.ui-spinner .btn:last-of-type').on('click',function(){");
                    script.Append("$('.ui-spinner input').val(parseInt($('.ui-spinner input').val(),10)-1);");
                script.Append("});");
            script.Append("})(jQuery);");
            return script.ToString();
        }

        public static object NoSuccess()
        {
            return new { success = false };
        }
        public static object Success()
        {
            return new { success = true };
        }
    }
}
