using System.Data.Entity.ModelConfiguration;
using Codestetic.Web.Core.Domain.Logging;

namespace Codestetic.Web.Data.Mapping.Logging
{
    public partial class PerformanceMap : EntityTypeConfiguration<Performance>
    {
        public PerformanceMap()
        {
            this.ToTable("Performance");
            this.HasKey(p => p.Id);
        }
    }
}