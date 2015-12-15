using System;
using System.Data.Entity.ModelConfiguration;
using Codestetic.Web.Core.Domain.Configuration;

namespace Codestetic.Web.Data.Mapping.Configuration
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(c => c.Id);
        }
    }
}