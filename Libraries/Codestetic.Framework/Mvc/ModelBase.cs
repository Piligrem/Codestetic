using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace Codestetic.Web.Framework.Mvc
{
    public interface IModelBase { }
    public interface IEntityModelBase { }

    public abstract partial class ModelBase : IModelBase
    {
        public ModelBase()
        {
            this.CustomProperties = new Dictionary<string, object>();
        }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }

    public abstract partial class EntityModelBase : ModelBase, IEntityModelBase
    {
        [ResourceDisplayName("Admin.Common.Entity.Fields.Id")]
        public virtual long Id { get; set; }
    }
}
