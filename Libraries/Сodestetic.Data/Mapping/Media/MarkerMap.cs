using System.Data.Entity.ModelConfiguration;
using Codestetic.Web.Core.Domain.Media;

namespace Codestetic.Web.Data.Mapping.Media
{
    public partial class MarkerMap : EntityTypeConfiguration<Marker>
    {
        public MarkerMap()
        {
            this.ToTable("Marker");
            this.HasKey(p => p.Id);
            this.Property(p => p.MarkerBinary).IsMaxLength();
            this.Property(p => p.MimeType).IsRequired().HasMaxLength(40);
            this.Property(p => p.FileName).HasMaxLength(300);

            // Device settings
            //this.HasMany(x => x.DeviceSettings)
            //    .WithMany()
            //    .Map(x => {
            //        x.ToTable("DeviceSetting");
            //        x.MapLeftKey("Id");
            //        x.MapRightKey("MarkerId");
            //    });
            //this.HasMany(x => x.DeviceSettings);
            //this.HasRequired(x => x.DeviceSettings)
            //    .WithMany();
        }
    }
}