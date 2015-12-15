using System.Data.Entity.ModelConfiguration;
using Codestetic.Web.Core.Domain.Media;

namespace Codestetic.Web.Data.Mapping.Media
{
    public partial class DownloadMap : EntityTypeConfiguration<Download>
    {
        public DownloadMap()
        {
            this.ToTable("Download");
            this.HasKey(p => p.Id);
            this.Property(p => p.DownloadBinary).IsMaxLength();
        }
    }
}