using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.WebPages;
using Kendo.Mvc.UI.Fluent;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Core.Localization;
using Codestetic.Web.Core.Domain.Common;
using System.Web.Routing;

namespace Codestetic.Web.Framework.UI
{
    public static class ScaffoldExtensions
    {
        public static string SymbolForBool<T>(this HtmlHelper<T> helper, string boolFieldName)
        {
            return "<i class='grid-icon grid-icon-active-#={0}#'></i>".FormatInvariant(boolFieldName);
        }
        public static MvcHtmlString SymbolForBool<T>(this HtmlHelper<T> helper, bool value, object htmlAttributes = null)
        {
            return SymbolForBool(helper, value, "icon-active-{0}".FormatInvariant(value.ToString().ToLower()), htmlAttributes);
        }
        public static MvcHtmlString SymbolForBool<T>(this HtmlHelper<T> helper, bool value, string symbol, object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder("i");
            tagBuilder.MergeAttribute("class", symbol);

            var attributes = (new RouteValueDictionary(htmlAttributes)).ToDictionary(x => x.Key, x => x.Value);
            tagBuilder.MergeAttributes<string, object>(attributes);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        public static HelperResult SymbolForBool<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var value = bool.Parse(metadata.SimpleDisplayText);
            return new HelperResult(writer => writer.Write("<i class='icon-active-{0}'></i>".FormatInvariant(value.ToString().ToLower())));
        }

		public static string LabeledProductName<T>(this HtmlHelper<T> helper, string id, string name, string typeName = "ProductTypeName", string typeLabelHint = "ProductTypeLabelHint")
		{
			string namePart = null;

			if (id.HasValue())
			{
				string url = UrlHelper.GenerateContentUrl("~/Admin/Product/Edit/", helper.ViewContext.RequestContext.HttpContext);

				namePart = "<a href=\"{0}<#= {1} #>\"><#= {2} #></a>".FormatInvariant(url, id, name);
			}
			else
			{
				namePart = "<span><#= {0} #></span>".FormatInvariant(name);
			}

			string result = "<span class='label label-smnet label-<#= {0} #>'><#= {1} #></span>{2}".FormatInvariant(typeLabelHint, typeName, namePart);
			
			return "<# if({0} && {0}.length > 0) {{ #>{1}<# }} #>".FormatInvariant(name, result);
		}
		public static HelperResult LabeledProductName<T>(this HtmlHelper<T> helper, int id, string name, string typeName, string typeLabelHint)
		{
			if (id == 0 && name.IsNullOrEmpty())
				return null;

			string namePart = null;

			if (id != 0)
			{
				string url = UrlHelper.GenerateContentUrl("~/Admin/Product/Edit/", helper.ViewContext.RequestContext.HttpContext);

				namePart = "<a href=\"{0}{1}\">{2}</a>".FormatInvariant(url, id, helper.Encode(name));
			}
			else
			{
				namePart = "<span>{0}</span>".FormatInvariant(helper.Encode(name));
			}

			return new HelperResult(writer => writer.Write("<span class='label label-smnet label-{0}'>{1}</span>{2}".FormatInvariant(typeLabelHint, typeName, namePart)));
		}

        public static string RichEditorFlavor(this HtmlHelper helper)
        {
            return EngineContext.Current.Resolve<AdminAreaSettings>().RichEditorFlavor.NullEmpty() ?? "RichEditor";
        }

        public static GridEditActionCommandBuilder Localize(this GridEditActionCommandBuilder builder, Localizer T)
        {
            return builder.Text(T("Admin.Common.Edit").Text)
                          .UpdateText(T("Admin.Common.Save").Text)
                          .CancelText(T("Admin.Common.Cancel").Text)
                          .Text(T("Admin.Telerik.GridLocalization.Insert").Text);
        }

        public static GridDestroyActionCommandBuilder Localize(this GridDestroyActionCommandBuilder builder, Localizer T)
        {
            return builder.Text(T("Admin.Common.Delete").Text);
        }
    }
}

