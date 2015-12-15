using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specter.Web.Core.Domain.Directory
{
    public class Panel : BaseEntity
    {
        #region Fields
        #endregion Fields

        #region Constructors
        public Panel() {}
        #endregion Constructors

        #region Properties
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Action { get; set; }
        public int TypeId { get; set; }
        [NotMapped]
        public PanelActionType Type
        {
            get { return (PanelActionType)TypeId; }
            set { this.TypeId = (int)value; }
        }
        public int DisplayOrder { get; set; }
        public bool Visible { get; set; }
        #endregion Properties

        #region Navigation properties
        public virtual ICollection<Panel> Children { get; set; }
        public virtual Panel ToolPanel { get; set; }
        #endregion Navigation properties
    }
}
