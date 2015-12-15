using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using System.Web;
using System.Threading;
using System.Web.Routing;
using System.Reflection;

using Codestetic.Web.Core;
using Codestetic.Web.Framework.Settings;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Framework.Localization;
using Codestetic.Web.Framework.Mvc;
using Codestetic.Web.Framework.UI;
using Codestetic.Web.Utilities;

namespace Codestetic.Web.Framework
{
    public enum InputEditorType
    {
        TextBox,
        Password,
        Hidden,
        Checkbox/*,
        RadioButton*/
    }

    public static class HtmlExtensions
    {
        #region Fields
        //private static Func<string> _formIdGenerator;
        #endregion Fields

        public static MvcHtmlString Hint(this HtmlHelper helper, string value)
        {
            // create a
            var a = new TagBuilder("a");
            a.MergeAttribute("href", "#");
            a.MergeAttribute("onclick", "return false;");
            a.MergeAttribute("rel", "tooltip");
            a.MergeAttribute("title", value);
            a.MergeAttribute("tabindex", "-1");
            a.AddCssClass("hint");

            // Create img
            var img = new TagBuilder("i");

            // Add attributes
            img.MergeAttribute("class", "icon-question-sign");

            a.InnerHtml = img.ToString();

            // Render tag
            return MvcHtmlString.Create(a.ToString());
        }

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this HtmlHelper<T> helper, string name,
             Func<int, HelperResult> localizedTemplate,
             Func<T, HelperResult> standardTemplate)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
            {
                if (helper.ViewData.Model.Locales.Count > 1)
                {
                    writer.Write("<div class='well well-small'>");
                    var tabStrip = helper.TabSite().TabStrip().Name(name).SmartTabSelection(false).Style(TabsStyle.Pills).Items(x =>
                    {
                        x.Add().Text("Standard").Content(standardTemplate(helper.ViewData.Model).ToHtmlString()).Selected(true);
                        for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                        {
                            var locale = helper.ViewData.Model.Locales[i];
                            var language = EngineContext.Current.Resolve<ILanguageService>().GetLanguageById(locale.LanguageId);
                            x.Add().Text(language.Name)
                                .Content(localizedTemplate
                                    (i).
                                    ToHtmlString
                                    ())
                                .ImageUrl("~/Content/images/flags/" + language.FlagImageFileName);
                        }
                    }).ToHtmlString();
                    writer.Write(tabStrip);
                    writer.Write("</div>");

                    #region OBSOLETE
                    //var tabStrip = helper.Telerik().TabStrip().Name(name).Items(x =>
                    //{
                    //    x.Add().Text("Standard").Content(standardTemplate(helper.ViewData.Model).ToHtmlString()).Selected(true);
                    //    for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                    //    {
                    //        var locale = helper.ViewData.Model.Locales[i];
                    //        var language = EngineContext.Current.Resolve<ILanguageService>().GetLanguageById(locale.LanguageId);
                    //        x.Add().Text(language.Name)
                    //            .Content(localizedTemplate
                    //                (i).
                    //                ToHtmlString
                    //                ())
                    //            .ImageUrl("~/Content/images/flags/" + language.FlagImageFileName);
                    //    }
                    //}).ToHtmlString();
                    //writer.Write(tabStrip);
                    #endregion
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static MvcHtmlString DeleteConfirmationSimple<T>(this HtmlHelper<T> helper, string handler = null, string buttonsSelector = null) where T : class
        {
            return DeleteConfirmationSimple<T>(helper, "", handler, buttonsSelector);
        }
        public static MvcHtmlString DeleteConfirmationSimple<T>(this HtmlHelper<T> helper, string actionName, string handler = null, string buttonsSelector = null) where T : class
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var localization = EngineContext.Current.Resolve<ILocalizationService>();
            var initiator = helper.ViewData.ModelMetadata.ModelType.Name.ToLower();
            var modalId = MvcHtmlString.Create(initiator + "-delete-confirmation").ToHtmlString();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = 0,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
                ActionName = actionName,
                ButtonSelector = buttonsSelector,
                // TODO: (MC) this is really awkward, but sufficient for the moment
                EntityType = buttonsSelector.Replace("-delete", "")
            };

            var window = helper.TabSite().Window()
                .Initiator(buttonsSelector)
                .Name(modalId)
                .Title(EngineContext.Current.Resolve<ILocalizationService>().GetResource("Common.AreYouSure"))
                .Modal(true)
                .Visible(false)
                .Draggable(true)
                .CloseOnEscapePress(true)
                .ShowAnimation(new VisualEffect() { Effect = EnumEffect.bounce, Delay = 500 })
                .HideAnimation(new VisualEffect() { Effect = EnumEffect.explode, Delay = 500 })
                .Buttons(new List<Button>() 
                { 
                    new Button() 
                    {
                        Title = localization.GetResource("Common.Delete"),
                        Action = !handler.IsNullOrEmpty() ? handler : (object)EnumActions.Unknow
                    },
                    new Button() 
                    {
                        Title = localization.GetResource("Common.NoCancel"),
                        Action = EnumActions.Close
                    }
                })
                .Content(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString())
                .ToHtmlString();

            return MvcHtmlString.Create(window); // + script);
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector = null) where T : EntityModelBase
        {
            return DeleteConfirmation<T>(helper, "", buttonsSelector);
        }
        // Adds an action name parameter for using other delete action names
        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string actionName, string buttonsSelector = null) where T : EntityModelBase
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var localization = EngineContext.Current.Resolve<ILocalizationService>();
            var initiator = helper.ViewData.ModelMetadata.ModelType.Name.ToLower();
            var modalId = MvcHtmlString.Create(initiator + "-delete-confirmation").ToHtmlString();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
                ActionName = actionName,
                ButtonSelector = buttonsSelector,
                // TODO: (MC) this is really awkward, but sufficient for the moment
                EntityType = buttonsSelector.Replace("-delete", "")
            };

            var window = helper.TabSite().Window()
                .Initiator(buttonsSelector)
                .Name(modalId)
                .Title(EngineContext.Current.Resolve<ILocalizationService>().GetResource("Common.AreYouSure"))
                .Modal(true)
                .Visible(false)
                .Draggable(true)
                .CloseOnEscapePress(true)
                .ShowAnimation(new VisualEffect() { Effect = EnumEffect.bounce, Delay = 500 })
                .HideAnimation(new VisualEffect() { Effect = EnumEffect.explode, Delay = 500 })
                .Buttons(new List<Button>() 
                { 
                    new Button() 
                    {
                        Title = localization.GetResource("Common.Delete"),
                        Action = EnumActions.Submit
                    },
                    new Button() 
                    {
                        Title = localization.GetResource("Common.NoCancel"),
                        Action = EnumActions.Close
                    }
                })
                .Content(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString())
                .ToHtmlString();

            return MvcHtmlString.Create(window); // + script);
        }

        public static MvcHtmlString ResetConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector = null) where T : EntityModelBase
        {
            return ResetConfirmation<T>(helper, "", buttonsSelector);
        }

        public static MvcHtmlString ResetConfirmation<T>(this HtmlHelper<T> helper, string actionName, string buttonsSelector = null) where T : EntityModelBase
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Reset";

            var modalId = MvcHtmlString.Create(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-reset-confirmation").ToHtmlString();

            string script = "";
            if (!string.IsNullOrEmpty(buttonsSelector))
            {
                script = "<script>$(function() { $('#" + modalId + "').modal({show:false}); $('#" + buttonsSelector + "').click( function(e){e.preventDefault();openModalWindow('" + modalId + "');} );  });</script>\n";
            }

            var resetConfirmationModel = new ResetConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
                ActionName = actionName,
                ButtonSelector = buttonsSelector,
                // TODO: (MC) this is really awkward, but sufficient for the moment
                EntityType = buttonsSelector.Replace("-reset", "")
            };

            var window = helper.TabSite().Window().Name(modalId)
                .Title(EngineContext.Current.Resolve<ILocalizationService>().GetResource("Admin.Common.AreYouSure"))
                .Modal(true)
                .Visible(false)
                .Content(helper.Partial("Reset", resetConfirmationModel).ToHtmlString())
                .ToHtmlString();

            return MvcHtmlString.Create(script + window);
        }

        #region ToolItem
        public static MvcHtmlString ToolItem<TEntity, TProperty>(this HtmlHelper<TEntity> helper, Expression<Func<TEntity, TProperty>> expression) where TEntity : EntityModelBase, IModelBase
        {
            var equality = expression.Body;
            var TType = equality.Type;

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var iconSpan = new TagBuilder("span");
            var tooltipFormat = metadata.Properties.SingleOrDefault(property => property.PropertyName.Equals("TooltipFormat")).SimpleDisplayText;
            var tooltipValue = metadata.Properties.SingleOrDefault(property => property.PropertyName.Equals("TooltipValue")).SimpleDisplayText;
            var iconFormat = metadata.Properties.SingleOrDefault(property => property.PropertyName.Equals("IconFormat")).SimpleDisplayText;
            var iconValue = metadata.Properties.SingleOrDefault(property => property.PropertyName.Equals("IconValue")).SimpleDisplayText;

            if (tooltipFormat.HasValue())
            {
                iconSpan.Attributes.Add("resource", tooltipFormat);
                iconSpan.Attributes.Add("title", tooltipFormat.FormatWith(tooltipValue));
            }
            if (iconFormat.HasValue())
                iconSpan.AddCssClass(iconFormat.FormatWith(iconValue));
            else
                iconSpan.AddCssClass(iconValue);

            return MvcHtmlString.Create(iconSpan.ToString());
        }
        #endregion ToolItem

        public static MvcHtmlString SpecterLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool displayHint = true, object htmlAttributes = null, string displayText = null)
        {
            var result = new StringBuilder();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            string labelText = null;
            string hint = null;

            ResourceDisplayName resourceDisplayName;
            object value = null;

            if (metadata.AdditionalValues.TryGetValue("ResourceDisplayName", out value))
            {
                resourceDisplayName = value as ResourceDisplayName;
                if (resourceDisplayName != null)
                {
                    // resolve label display name
                    labelText = resourceDisplayName.DisplayName.NullEmpty();
                    if (labelText == null)
                    {
                        // take res key as absolute fallback
                        labelText = resourceDisplayName.ResourceKey;
                    }

                    // resolve hint
                    if (displayHint)
                    {
                        var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                        hint = EngineContext.Current.Resolve<ILocalizationService>()
                            .GetResource(resourceDisplayName.ResourceKey + ".Hint", langId, false, "", true);
                    }
                }
            }

            if (labelText == null)
            {
                labelText = metadata.PropertyName.SplitPascalCase();
            }

            var label = helper.LabelFor(expression, displayText ?? labelText, htmlAttributes);

            if (displayHint)
            {
                result.Append("<div class='ctl-label'>");
                {
                    result.Append(label);
                    if (hint.HasValue())
                    {
                        result.Append(helper.Hint(hint).ToHtmlString());
                    }
                }
                result.Append("</div>");
            }
            else
            {
                result.Append(label);
            }

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString SpecterLabel<TModel>(this HtmlHelper<TModel> helper, string resourceKey, object htmlAttributes = null)
        {
            var result = new StringBuilder();
            var label = helper.Label(resourceKey, htmlAttributes);
            result.Append("<div class='ctl-label'>");
            result.Append(label);
            result.Append("</div>");
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString SpecterEditFromToFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expressionFrom, Expression<Func<TModel, TValue>> expressionTo)
        {
            var result = new StringBuilder();
            var metadataFrom = ModelMetadata.FromLambdaExpression(expressionFrom, helper.ViewData);
            var metadataTo = ModelMetadata.FromLambdaExpression(expressionTo, helper.ViewData);

            var editorFrom = helper.EditorFor(expressionFrom).ToString().Replace("t-input", "t-input t-sizefromto");
            result.Append(editorFrom);

            result.Append("<a class=\"t-separatingline\">-</a>");

            var editorTo = helper.EditorFor(expressionTo).ToString().Replace("t-input", "t-input t-sizefromto");
            result.Append(editorTo);

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RequiredHint(this HtmlHelper helper, string additionalText = null)
        {
            // Create tag builder
            var builder = new TagBuilder("span");
            builder.AddCssClass("required");
            var innerText = "*";
            //add additional text if specified
            if (!String.IsNullOrEmpty(additionalText))
                innerText += " " + additionalText;
            builder.SetInnerText(innerText);
            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static string FieldNameFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }

        public static string FieldIdFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
        }

        /// <summary>
        /// Creates a days, months, years drop down list using an HTML select control. 
        /// The parameters represent the value of the "name" attribute on the select control.
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="dayName">"Name" attribute of the day drop down list.</param>
        /// <param name="monthName">"Name" attribute of the month drop down list.</param>
        /// <param name="yearName">"Name" attribute of the year drop down list.</param>
        /// <param name="beginYear">Begin year</param>
        /// <param name="endYear">End year</param>
        /// <param name="selectedDay">Selected day</param>
        /// <param name="selectedMonth">Selected month</param>
        /// <param name="selectedYear">Selected year</param>
        /// <param name="localizeLabels">Localize labels</param>
        /// <returns></returns>
        public static MvcHtmlString DatePickerDropDowns(this HtmlHelper html,
            string dayName, string monthName, string yearName,
            int? beginYear = null, int? endYear = null,
            int? selectedDay = null, int? selectedMonth = null, int? selectedYear = null, bool localizeLabels = true, bool disabled = false)
        {
            var daysList = new TagBuilder("select");
            daysList.MergeAttribute("style", "width: 70px");
            var monthsList = new TagBuilder("select");
            monthsList.MergeAttribute("style", "width: 130px");
            var yearsList = new TagBuilder("select");
            yearsList.MergeAttribute("style", "width: 90px");

            daysList.Attributes.Add("name", dayName);
            monthsList.Attributes.Add("name", monthName);
            yearsList.Attributes.Add("name", yearName);

            daysList.Attributes.Add("class", "date-part");
            monthsList.Attributes.Add("class", "date-part");
            yearsList.Attributes.Add("class", "date-part");

            if (disabled)
            {
                daysList.Attributes.Add("disabled", "disabled");
                monthsList.Attributes.Add("disabled", "disabled");
                yearsList.Attributes.Add("disabled", "disabled");
            }

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale, monthLocale, yearLocale;
            if (localizeLabels)
            {
                var locService = EngineContext.Current.Resolve<ILocalizationService>();
                dayLocale = locService.GetResource("Common.Day");
                monthLocale = locService.GetResource("Common.Month");
                yearLocale = locService.GetResource("Common.Year");
            }
            else
            {
                dayLocale = "Day";
                monthLocale = "Month";
                yearLocale = "Year";
            }

            days.AppendFormat("<option>{0}</option>", dayLocale);
            for (int i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedDay.HasValue && selectedDay.Value == i) ? " selected=\"selected\"" : null);


            months.AppendFormat("<option>{0}</option>", monthLocale);
            for (int i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                                    i,
                                    (selectedMonth.HasValue && selectedMonth.Value == i) ? " selected=\"selected\"" : null,
                                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }


            years.AppendFormat("<option>{0}</option>", yearLocale);

            if (beginYear == null)
                beginYear = DateTime.UtcNow.Year - 90;
            if (endYear == null)
                endYear = DateTime.UtcNow.Year + 10;

            for (int i = beginYear.Value; i <= endYear.Value; i++)
                years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);

            daysList.InnerHtml = days.ToString();
            monthsList.InnerHtml = months.ToString();
            yearsList.InnerHtml = years.ToString();

            return MvcHtmlString.Create(string.Concat(daysList, monthsList, yearsList));
        }

        #region DropDownList Extensions

        private static readonly SelectListItem[] _singleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        public static MvcHtmlString DropDownListForEnum<TModel, TEnum>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            string optionLabel = null) where TEnum : struct
        {

            return htmlHelper.DropDownListForEnum(expression, null, optionLabel);
        }

        public static MvcHtmlString DropDownListForEnum<TModel, TEnum>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            object htmlAttributes,
            string optionLabel = null) where TEnum : struct
        {
            IDictionary<string, object> attrs = null;
            if (htmlAttributes != null)
            {
                attrs = CollectionHelper.ObjectToDictionary(htmlAttributes);
            }

            return htmlHelper.DropDownListForEnum(expression, attrs, optionLabel);
        }

        public static MvcHtmlString DropDownListForEnum<TModel, TEnum>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            IDictionary<string, object> htmlAttributes,
            string optionLabel = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("An Enumeration type is required.", "expression");

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = metadata.ModelType.GetNonNullableType();
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                values.Select(value => new SelectListItem
                {
                    Text = value.GetLocalizedEnum(localizationService, workContext),
                    Value = Enum.GetName(enumType, value),
                    Selected = value.Equals(metadata.Model.Convert(enumType))
                });

            if (metadata.IsNullableValueType)
            {
                items = _singleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(expression, items, optionLabel, htmlAttributes);
        }

        #endregion

        public static MvcHtmlString Widget(this HtmlHelper helper, string widgetZone)
        {
            if (widgetZone.HasValue())
            {
                var widgetSelector = EngineContext.Current.Resolve<IWidgetSelector>();
                var widgets = widgetSelector.GetWidgets(widgetZone);

                if (widgets.Any())
                {
                    var result = helper.Action("WidgetsByZone", "Widget", new { widgets = widgets });
                    return result;
                }
            }

            return MvcHtmlString.Create("");
        }

        public static IHtmlString MetaAcceptLanguage(this HtmlHelper html)
        {
            var acceptLang = HttpUtility.HtmlAttributeEncode(Thread.CurrentThread.CurrentUICulture.ToString());
            return new HtmlString(string.Format("<meta name=\"accept-language\" content=\"{0}\"/>", acceptLang));
        }

        public static MvcHtmlString ControlGroupFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            InputEditorType editorType = InputEditorType.TextBox,
            bool required = false,
            string helpHint = null)
        {
            if (editorType == InputEditorType.Hidden)
            {
                return html.HiddenFor(expression);
            }

            var sb = new StringBuilder("<div class='control-group'>");

            if (editorType != InputEditorType.Checkbox)
            {
                var className = "control-label" + (required ? " required" : "");
                var fieldId = html.IdFor(expression).ToString();
                sb.AppendLine(html.LabelFor(expression, new { @class = className, @for = fieldId }).ToString());
            }

            sb.AppendLine("<div class='controls'>");
            string inputHtml = "";
            object attrs = null;
            if (!required && (editorType == InputEditorType.TextBox || editorType == InputEditorType.Password))
            {
                attrs = new { placeholder = "Optional" /* TODO: Loc */  };
            }
            //var x = ModelMetadata.FromLambdaExpression(expression, html.ViewData).DisplayName;
            switch (editorType)
            {
                case InputEditorType.Checkbox:
                    inputHtml = string.Format("<label class='checkbox'>{0} {1}</label>",
                        html.EditorFor(expression).ToString(),
                        ModelMetadata.FromLambdaExpression(expression, html.ViewData).DisplayName); // TBD: ist das OK so?
                    break;
                case InputEditorType.Password:
                    inputHtml = html.PasswordFor(expression, attrs).ToString();
                    break;
                default:
                    inputHtml = html.TextBoxFor(expression, attrs).ToString();
                    break;
            }
            sb.AppendLine(inputHtml);
            sb.AppendLine(html.ValidationMessageFor(expression).ToString());
            if (helpHint.HasValue())
            {
                sb.AppendLine(string.Format("<div class='help-block muted'>{0}</div>", helpHint));
            }
            sb.AppendLine("</div>"); // div.controls

            sb.AppendLine("</div>"); // div.control-group

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ColorBox(this HtmlHelper html, string name, string color)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<div class='input-append color sm-colorbox' data-color='{0}' data-color-format='hex'>", color);

            sb.AppendFormat(html.TextBox(name, color, new { @class = "span2 colorval" }).ToHtmlString());
            sb.AppendFormat("<span class='add-on'><i style='background-color:{0}; border:1px solid #bbb'></i></span>", color);

            sb.Append("</div>");

            var bootstrapJsRoot = "~/Content/bootstrap/js/";
            html.AppendScriptParts(false,
                bootstrapJsRoot + "custom/bootstrap-colorpicker.js",
                bootstrapJsRoot + "custom/bootstrap-colorpicker-globalinit.js");

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString TableFormattedVariantAttributes(this HtmlHelper helper, string formattedVariantAttributes, string separatorLines = "<br />", string separatorValues = ": ")
        {
            var sb = new StringBuilder();
            string name, value;
            string[] lines = formattedVariantAttributes.SplitSafe(separatorLines);

            if (lines.Length <= 0)
                return MvcHtmlString.Empty;

            sb.Append("<table class=\"product-attribute-table\">");

            foreach (string line in lines)
            {
                sb.Append("<tr>");
                if (line.SplitToPair(out name, out value, separatorValues))
                {
                    sb.AppendFormat("<td class=\"column-name\">{0}:</td>", name);
                    sb.AppendFormat("<td class=\"column-value\">{0}</td>", value);
                }
                else
                {
                    sb.AppendFormat("<td colspan=\"2\">{0}</td>", line);
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString SettingOverrideCheckbox<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string parentSelector = null)
        {
            var data = helper.ViewData[DependingSettingHelper.ViewDataKey] as DependingSettingData;

            if (data != null && data.OverrideSettingKeys.Count > 0)
            {
                var settingKey = ExpressionHelper.GetExpressionText(expression);
                var localizeService = EngineContext.Current.Resolve<ILocalizationService>();

                if (!settingKey.Contains("."))
                    settingKey = data.RootSettingClass + "." + settingKey;

                var overrideForSite = (data.OverrideSettingKeys.FirstOrDefault(x => x.IsCaseInsensitiveEqual(settingKey)) != null);
                var fieldId = settingKey + (settingKey.EndsWith("_OverrideForSite") ? "" : "_OverrideForSite");

                var sb = new StringBuilder();
                sb.Append("<div class=\"onoffswitch-container\"><div class=\"onoffswitch\">");

                sb.AppendFormat("<input type=\"checkbox\" id=\"{0}\" name=\"{0}\" class=\"onoffswitch-checkbox multi-store-override-option\"", fieldId);
                sb.AppendFormat(" onclick=\"Admin.checkOverriddenSiteValue(this)\" data-parent-selector=\"{0}\"{1} />", parentSelector.EmptyNull(), overrideForSite ? " checked=\"checked\"" : "");

                sb.AppendFormat("<label class=\"onoffswitch-label\" for=\"{0}\">", fieldId);
                sb.AppendFormat("<span class=\"onoffswitch-on\">{0}</span>", localizeService.GetResource("Common.On").Truncate(3).ToUpper());
                sb.AppendFormat("<span class=\"onoffswitch-off\">{0}</span>", localizeService.GetResource("Common.Off").Truncate(3).ToUpper());
                sb.Append("<span class=\"onoffswitch-switch\"></span>");
                sb.Append("<span class=\"onoffswitch-inner\"></span>");
                sb.Append("</label>");
                sb.Append("</div></div>\r\n");		// controls are not floating, so line-break prevents different distances between them

                return MvcHtmlString.Create(sb.ToString());
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString SettingEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string parentSelector = null, bool overrideCheckbox = false)
        {
            var checkbox = MvcHtmlString.Empty;
            if (overrideCheckbox)
                checkbox = helper.SettingOverrideCheckbox(expression, parentSelector);

            var editor = helper.EditorFor(expression);

            return MvcHtmlString.Create(checkbox.ToString() + editor.ToString());
        }

        public static MvcHtmlString Video(this HtmlHelper helper,
            string id, string source, string notSupportedMessage, bool showControls, bool autoPlay, bool playInLoop, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("video");
            var url = string.Empty;
            if (!string.IsNullOrEmpty(source))
                url = UrlHelper.GenerateContentUrl(source, helper.ViewContext.HttpContext);

            if (htmlAttributes != null)
            {
                RouteValueDictionary routeValueDictionary = new RouteValueDictionary(htmlAttributes);
                tagBuilder.MergeAttributes(routeValueDictionary);
            }
            if (showControls)
                tagBuilder.MergeAttribute("controls", "controls");
            if (autoPlay)
                tagBuilder.MergeAttribute("autoplay", "autoplay");
            if (playInLoop)
                tagBuilder.MergeAttribute("loop", "loop");
            tagBuilder.MergeAttribute("src", url);
            tagBuilder.MergeAttribute("id", id);
            tagBuilder.InnerHtml = notSupportedMessage;

            var webmSourceTag = new TagBuilder("source");
            webmSourceTag.MergeAttribute("id", "webm");
            webmSourceTag.MergeAttribute("src", url + ".webm");
            webmSourceTag.MergeAttribute("type", "video/webm");

            var mp4SourceTag = new TagBuilder("source");
            mp4SourceTag.MergeAttribute("id", "mp4");
            mp4SourceTag.MergeAttribute("src", url + ".mp4");
            mp4SourceTag.MergeAttribute("type", "video/mp4");

            var oggSourceTag = new TagBuilder("source");
            oggSourceTag.MergeAttribute("id", "ogg");
            oggSourceTag.MergeAttribute("src", url + ".ogg");
            oggSourceTag.MergeAttribute("type", "video/ogg");

            tagBuilder.InnerHtml += webmSourceTag.ToString(TagRenderMode.Normal);
            tagBuilder.InnerHtml += mp4SourceTag.ToString(TagRenderMode.Normal);
            tagBuilder.InnerHtml += oggSourceTag.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        #region LinkTable/RouteTable/Form
        public static MvcHtmlString RouteLinkTooltip(this HtmlHelper helper, string linkText, string comment, object routeName)
        {
            return RouteLinkTooltip(helper, linkText, comment, routeName, null);
        }
        public static MvcHtmlString RouteLinkTooltip(this HtmlHelper helper, string linkText, string comment, object routeName, object htmlAttributes)
        {
            var sb = new StringBuilder();

            if (routeName.ToString().IsNullOrEmpty())
            {
                sb.Append("<a href='javascript:void(0)' data-target='#'>");
            }
            else
            {
                var routeLink = LinkExtensions.RouteLink(helper, linkText, routeName: routeName.ToString(), routeValues: null, htmlAttributes: htmlAttributes).ToHtmlString(); // string.Format("<a href=\"/{0}\" id=\"{1}\">{2}", routeValue, "", linkText);
                sb.Append(routeLink.Substring(0, routeLink.IndexOf(">" + linkText) + 1));
            }
            if (!string.IsNullOrEmpty(comment))
            {
                sb.Append("<div>");
                sb.Append(linkText);
                sb.Append("<span>" + comment + "</span>");
                sb.Append("</div>");
            }
            else
            {
                sb.Append(linkText);
            }
            sb.Append("</a>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString LinkTable(this HtmlHelper helper, string linkText, string icon, string comment, string actionName, string controllerName)
        {
            return LinkTable(helper, linkText, icon, comment, actionName, controllerName, null, null);
        }
        public static MvcHtmlString LinkTable(this HtmlHelper helper, string linkText, string comment, string actionName, string controllerName)
        {
            return LinkTable(helper, linkText, null, comment, actionName, controllerName, null, null);
        }
        public static MvcHtmlString LinkTable(this HtmlHelper helper, string linkText, string comment, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return LinkTable(helper, linkText, null, comment, actionName, controllerName, routeValues, htmlAttributes);
        }
        public static MvcHtmlString LinkTable(this HtmlHelper helper, string linkText, string icon, string comment, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(actionName) && string.IsNullOrEmpty(controllerName))
            {
                sb.Append("<a href='javascript:void(0)' data-target='#'>");
            }
            else
            {
                var actionLink = LinkExtensions.ActionLink(helper, linkText, actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
                sb.Append(actionLink.Substring(0, actionLink.IndexOf(">" + linkText) + 1));
            }
            if (!string.IsNullOrEmpty(comment))
            {
                sb.Append("<div>");
                sb.Append(linkText);
                sb.Append("<span>" + comment + "</span>");
                sb.Append("</div>");
            }
            else
            {
                if (!string.IsNullOrEmpty(icon))
                {
                    sb.Append("<i class='" + icon + "'></i>");
                    sb.Append("<h3>" + linkText + "</h3>");
                }
                else
                {
                    sb.Append(linkText);
                }
            }
            sb.Append("</a>");
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcForm BeginForm(this HtmlHelper helper, object routeName, object routeValues, FormMethod method, object htmlAttributes)
        {
            var builder = new TagBuilder("form");
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            builder.MergeAttributes<string, object>(attributes);
            builder.MergeAttribute("action", routeName.ToString());
            builder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            var flag = helper.ViewContext.ClientValidationEnabled && !helper.ViewContext.UnobtrusiveJavaScriptEnabled;
            if (flag)
            {
                //builder.GenerateId(helper.ViewContext.FormIdGenerator());
            }
            helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag));
            var form = new MvcForm(helper.ViewContext);
            if (flag)
            {
                //helper.ViewContext.FormContext.FormId = builder.Attributes["id"];
            }
            return form;
        }
        #endregion LinkTable

        public static MvcHtmlString Image<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var tagBuilder = new TagBuilder("img");
            //tagBuilder.MergeAttribute()
            //var image = helper.Image<TModel, TValue>(expression, htmlAttributes);
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            tagBuilder.MergeAttribute("src", (string)metadata.Model);

            var attributes = (new RouteValueDictionary(htmlAttributes)).ToDictionary(x => x.Key.Replace("_", "-"), x => x.Value);
            tagBuilder.MergeAttributes<string, object>(attributes);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        #region Utilities
        #endregion Utilities
    }
}
