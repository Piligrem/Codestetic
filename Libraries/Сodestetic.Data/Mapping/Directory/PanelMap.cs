using Codestetic.Web.Core.Domain.Directory;
using System.Data.Entity.ModelConfiguration;

namespace Codestetic.Web.Data.Mapping.Directory
{
    public partial class PanelMap : EntityTypeConfiguration<Panel>
    {
        public PanelMap()
        {
            this.ToTable("Panel");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(100);
        }
    }
}
