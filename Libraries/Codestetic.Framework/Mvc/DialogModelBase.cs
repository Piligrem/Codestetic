namespace Codestetic.Web.Framework.Mvc
{
    public class DialogModelBase : EntityModelBase
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ButtonSelector { get; set; }
        public string EntityType { get; set; }
    }
}
