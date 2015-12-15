namespace Codestetic.Web.Framework.Mvc
{
    public class IconModel<T> : ModelBase
    {
        public string IconFormat { get; set; }
        public string IconValue { get; set; }
        public string TooltipFormat { get; set; }
        public T TooltipValue { get; set; }
    }
}
